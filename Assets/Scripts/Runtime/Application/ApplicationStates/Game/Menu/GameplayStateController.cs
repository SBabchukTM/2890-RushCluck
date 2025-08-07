using System.Threading;
using Core.StateMachine;
using Runtime.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;
using Core;
using Zenject;
using Runtime.Services;
using Core.UI;
using UnityEngine;
using Runtime.Services.UserData;
using System.Linq;
using Core.Services.Audio;
using Runtime.Services.Audio;

namespace Runtime.Game
{
    public class GameplayStateController : StateController, IInitializable
    {
        private readonly IUiService _uiService;
        private readonly IAudioService _audioService;
        private readonly IAssetProvider _assetProvider;
        private readonly ItemController _itemController;
        private readonly BalloonController _balloonController;
        private readonly UserDataService _userDataService;

        private GameplayScreen _screen;
        private GameConfig _gameConfig;
        private ShopConfig _shopConfig;

        private bool _gameEnded = false;
        private int _level;
        private int _collectedCoins = 0;
        private float _distanceTravelled = 0;
        private CancellationTokenSource _cancellationTokenSource;

        public GameplayStateController(ILogger logger, IUiService uiService, IAudioService audioService, ItemController itemController, UserDataService userDataService, IAssetProvider assetProvider, BalloonController balloonController) : base(logger)
        {
            _uiService = uiService;
            _audioService = audioService;
            _itemController = itemController;
            _assetProvider = assetProvider;
            _balloonController = balloonController;
            _userDataService = userDataService;
        }

        public async void Initialize()
        {
            _gameConfig = await _assetProvider.Load<GameConfig>(ConstConfigs.GameConfig);
            _shopConfig = await _assetProvider.Load<ShopConfig>(ConstConfigs.ShopItemsConfig);
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            ResetData();

            CreateScreen();
            SubscribeToEvents();
            StartGame();
            return UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            EndGame();

            _balloonController.OnCollected -= AddCollectible;
            _balloonController.OnCollided -= ShowLosePopup;
            _screen.OnPausePressed -= ShowPausePopup;

            await _uiService.HideScreen(ConstScreens.GameplayScreen);
        }

        public void SetLevel(int level) => _level = level;

        private void ResetData()
        {
            _gameEnded = false;
            _cancellationTokenSource = new();
            _collectedCoins = 0;
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<GameplayScreen>(ConstScreens.GameplayScreen);
            _screen.ShowAsync().Forget();
            _screen.Initialize();
        }

        private void SubscribeToEvents()
        {
            _balloonController.OnCollected += AddCollectible;
            _balloonController.OnCollided += ShowLosePopup;
            _screen.OnPausePressed += ShowPausePopup;
        }

        private void StartGame()
        {
            _itemController.StartGame(_gameConfig.LevelConfigs[_level], _cancellationTokenSource.Token);

            // _balloonController.SetSprite(GetSelectedSprite());
            _balloonController.gameObject.SetActive(true);
            _balloonController.transform.position = _gameConfig.BalloonSpawnPos;

            RunDistanceMeter(_cancellationTokenSource.Token).Forget();
        }

        private Sprite GetSelectedSprite()
        {
            return _shopConfig.ShopItems.Where(x => x.ItemID == _userDataService.GetUserData().UserInventory.UsedGameItemID).First().ItemSprite;
        }

        private void EndGame()
        {
            _balloonController.gameObject.SetActive(false);
            _itemController.EndGame();
        }

        private async UniTaskVoid RunDistanceMeter(CancellationToken token)
        {
            _distanceTravelled = 0;
            float targetDistance = _gameConfig.LevelConfigs[_level].TargetDistance;
            float speed = _gameConfig.LevelConfigs[_level].ScrollSpeed;

            while (!token.IsCancellationRequested && _distanceTravelled < targetDistance)
            {
                _distanceTravelled += speed * Time.deltaTime;
                _screen.UpdateProgress((int)_distanceTravelled, (int)targetDistance);
                await UniTask.NextFrame();
                token.ThrowIfCancellationRequested();
            }

            if (_distanceTravelled > targetDistance)
            {
                _audioService.PlaySound(ConstAudio.VictorySound);
                _userDataService.SaveUserLevelClearProgress(_level, _gameConfig.LevelConfigs.Count);
                ShowWinPopup();
            }
        }

        private async void ShowWinPopup()
        {
            if (_gameEnded)
                return;

            _gameEnded = true;
            Time.timeScale = 0;
            WinPopup popup = await _uiService.ShowPopup(ConstPopups.WinPopup) as WinPopup;

            popup.SetData(_collectedCoins);

            popup.OnHomePressed += async () =>
            {
                Time.timeScale = 1;
                popup.DestroyPopup();
                await GoTo<MenuStateController>();
            };

            popup.OnRestartPressed += async () =>
            {
                Time.timeScale = 1;
                popup.DestroyPopup();
                await GoTo<GameplayStateController>();
            };

            popup.OnContinuePressed += async () =>
            {
                Time.timeScale = 1;
                popup.DestroyPopup();

                if (_level + 1 < _gameConfig.LevelConfigs.Count)
                    SetLevel(_level + 1);

                await GoTo<GameplayStateController>();
            };
        }

        private async void ShowLosePopup()
        {
            if (_gameEnded)
                return;

            _gameEnded = true;

            _audioService.PlaySound(ConstAudio.PopSound);

            Time.timeScale = 0;
            LosePopup popup = await _uiService.ShowPopup(ConstPopups.LosePopup) as LosePopup;

            popup.SetData((int)_distanceTravelled);

            popup.OnHomePressed += async () =>
            {
                Time.timeScale = 1;
                popup.DestroyPopup();
                await GoTo<MenuStateController>();
            };

            popup.OnRestartPressed += async () =>
            {
                Time.timeScale = 1;
                popup.DestroyPopup();
                await GoTo<GameplayStateController>();
            };
        }

        private async void ShowPausePopup()
        {
            Time.timeScale = 0;
            PausePopup popup = await _uiService.ShowPopup(ConstPopups.PausePopup) as PausePopup;

            popup.OnContinuePressed += () =>
            {
                Time.timeScale = 1;
                popup.DestroyPopup();
            };

            popup.OnHomePressed += async () =>
            {
                Time.timeScale = 1;
                popup.DestroyPopup();
                await GoTo<MenuStateController>();
            };

            popup.OnRestartPressed += async () =>
            {
                Time.timeScale = 1;
                popup.DestroyPopup();
                await GoTo<GameplayStateController>();
            };
        }

        private void AddCollectible(GameObject obj)
        {
            _audioService.PlaySound(ConstAudio.CoinSound);

            _collectedCoins++;
            _screen.UpdateCollectibles(_collectedCoins);

            var userData = _userDataService.GetUserData();
            userData.UserInventory.Balance += 1;
            userData.UserProgressData.Score += 1;

            _itemController.ReturnCollectibleToPool(obj);
        }
    }
}
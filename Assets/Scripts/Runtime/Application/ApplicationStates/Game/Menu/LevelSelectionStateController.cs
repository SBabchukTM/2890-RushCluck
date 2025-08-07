using System.Threading;
using Core.StateMachine;
using Runtime.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;
using Runtime.Services.UserData;

namespace Runtime.Game
{
    public class LevelSelectionStateController : StateController
    {
        private readonly IUiService _uiService;
        private readonly UserDataService _userDataService;
        private readonly GameplayStateController _gameplayStateController;

        private LevelSelectionScreen _screen;

        private int _selectedLevelID = 0;

        public LevelSelectionStateController(ILogger logger, IUiService uiService, UserDataService userDataService, GameplayStateController gameplayStateController) : base(logger)
        {
            _uiService = uiService;
            _userDataService = userDataService;
            _gameplayStateController = gameplayStateController;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            _selectedLevelID = GetSelectedID();
            CreateScreen();
            SubscribeToEvents();
            return UniTask.CompletedTask;
        }

        private int GetSelectedID()
        {
            return _userDataService.GetUserData().UserProgressData.LastUnlockedLevelID;
        }

        public override async UniTask Exit()
        {
            await _uiService.HideScreen(ConstScreens.LevelSelectionScreen);
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<LevelSelectionScreen>(ConstScreens.LevelSelectionScreen);
            _screen.Initialize(_selectedLevelID);
            _screen.ShowAsync().Forget();
        }

        private void SubscribeToEvents()
        {
            _screen.OnBackPressed += async () => await GoTo<MenuStateController>();
            _screen.OnSelectedLevelChanged += (level) => _selectedLevelID = level;
            _screen.OnPlayPressed += async () =>
            {
                _gameplayStateController.SetLevel(_selectedLevelID);
                await GoTo<GameplayStateController>();
            };
        }
    }
}
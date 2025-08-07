using System.Threading;
using Core.StateMachine;
using Runtime.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;
using Core;
using Runtime.Services;
using System.Collections.Generic;
using Core.Factory;
using Runtime.Services.UserData;
using UnityEngine;
using Core.UI;
using Core.Services.Audio;
using Runtime.Services.Audio;
using Zenject;

namespace Runtime.Game
{
    public class ShopStateController : StateController, IInitializable
    {
        /*        private const string PurchaseStatus = "Purchase";
                private const string SelectedStatus = "Selected";
                private const string NotSelectedStatus = "Select";*/

        private readonly IUiService _uiService;
        private readonly IAssetProvider _assetProvider;
        private readonly IAudioService _audioService;
        private readonly GameObjectFactory _gameObjectFactory;
        private readonly UserDataService _userDataService;

        private ShopConfig _shopConfig;
        private UserInventory _userInventory;
        private GameObject _shopItemDisplayPrefab;

        private ShopScreen _screen;

        private List<ShopItemDisplay> _shopItemDisplayList;

        private CancellationTokenSource _cancellationTokenSource;

        public ShopStateController(ILogger logger, IUiService uiService, IAssetProvider assetProvider, IAudioService audioService, GameObjectFactory gameObjectFactory, UserDataService userDataService) : base(logger)
        {
            _uiService = uiService;
            _assetProvider = assetProvider;
            _gameObjectFactory = gameObjectFactory;
            _userDataService = userDataService;
            _audioService = audioService;
        }

        public async void Initialize()
        {
            _shopConfig = await _assetProvider.Load<ShopConfig>(ConstConfigs.ShopItemsConfig);
            _shopItemDisplayPrefab = await _assetProvider.Load<GameObject>(ConstPrefabs.ShopItemDisplayPrefab);
            _userInventory = _userDataService.GetUserData().UserInventory;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new();

            CreateShopItems();
            CreateScreen();
            SubscribeToEvents();

            return UniTask.CompletedTask;
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<ShopScreen>(ConstScreens.ShopScreen);
            _screen.Initialize();
            _screen.ShowAsync().Forget();
            _screen.UpdateBalance(_userInventory.Balance);
            _screen.SetShopItems(_shopItemDisplayList);
        }

        private void SubscribeToEvents()
        {
            _screen.OnBackPressed += async () => await GoTo<MenuStateController>();
        }

        public override async UniTask Exit()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            await _uiService.HideScreen(ConstScreens.ShopScreen);
        }

        private void CreateShopItems()
        {
            int size = _shopConfig.ShopItems.Count;

            _shopItemDisplayList = new(size);

            List<ShopItem> shopItems = _shopConfig.ShopItems;

            for (int i = 0; i < size; i++)
            {
                ShopItemDisplay shopItemDisplay = _gameObjectFactory.Create<ShopItemDisplay>(_shopItemDisplayPrefab);

                shopItemDisplay.Initialize(shopItems[i]);
                SetShopItemStatus(shopItemDisplay);

                _shopItemDisplayList.Add(shopItemDisplay);
                shopItemDisplay.OnPurchasePressed += ProcessPurchaseAttempt;
            }
        }

        private void UpdateAllShopItemsStatus()
        {
            foreach (var item in _shopItemDisplayList)
                SetShopItemStatus(item);
        }

        private void SetShopItemStatus(ShopItemDisplay shopItemDisplay)
        {
            //  bool selected = _userInventory.UsedGameItemID == shopItemDisplay.ShopItem.ItemID;

            /*            if (selected)
                        {
                            shopItemDisplay.SetStatus(SelectedStatus);
                            return;
                        }*/

            bool purchased = _userInventory.PurchasedGameItemsIDs.Contains(shopItemDisplay.ShopItem.ItemID);
            shopItemDisplay.SetStatus(purchased ? true : false);
        }

        private void ProcessPurchaseAttempt(ShopItemDisplay shopItemDisplay)
        {
            ShopItem shopItem = shopItemDisplay.ShopItem;

            if (_userInventory.PurchasedGameItemsIDs.Contains(shopItem.ItemID))
            {
                _userInventory.UsedGameItemID = shopItem.ItemID;
                UpdateAllShopItemsStatus();

                if (_shopConfig.PurchaseEffectSettings.PlaySoundOnSelectPurchased)
                    _audioService.PlaySound(ConstAudio.SelectSound);
            }
            else
            {
                if (_userInventory.Balance < shopItem.ItemPrice)
                {
                    ShowMessagePopup();

                    if (_shopConfig.PurchaseEffectSettings.ShakeIfNotEnoughCurrency)
                        shopItemDisplay.Shake(_cancellationTokenSource.Token, _shopConfig.PurchaseEffectSettings.PurchaseFailedShakeParameters).Forget();

                    if (_shopConfig.PurchaseEffectSettings.PlaySoundOnNotEnoughCurrency)
                        _audioService.PlaySound(ConstAudio.ErrorSound);

                    return;
                }

                ProcessPurchase(shopItem);
            }
        }

        private async void ProcessPurchase(ShopItem shopItem)
        {
            ItemPurchasePopup popup = await _uiService.ShowPopup(ConstPopups.ItemPurchasePopup,
                                                                 new ItemPurchasePopupData { ShopItem = shopItem })
                                                                 as ItemPurchasePopup;
            popup.OnAcceptPressedEvent += () =>
            {
                UpdateBalanceAfterPurchase(shopItem);

                if (_shopConfig.PurchaseEffectSettings.PlaySoundOnPurchase)
                    _audioService.PlaySound(ConstAudio.PurchaseSound);

                UpdateAllShopItemsStatus();

                _screen.UpdateBalance(_userInventory.Balance);

                popup.DestroyPopup();
            };

            popup.OnDenyPressedEvent += () => popup.DestroyPopup();
        }

        private void UpdateBalanceAfterPurchase(ShopItem shopItem)
        {
            int balance = _userInventory.Balance;
            balance -= shopItem.ItemPrice;

            _userInventory.Balance = balance;
            _userInventory.PurchasedGameItemsIDs.Add(shopItem.ItemID);
            _userInventory.UsedGameItemID = shopItem.ItemID;
            _userDataService.SaveUserData();
        }

        private void ShowMessagePopup()
        {
            _uiService.ShowPopup(ConstPopups.MessagePopup);
        }
    }
}
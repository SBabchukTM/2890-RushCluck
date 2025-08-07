using System.Threading;
using Core.StateMachine;
using Runtime.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;
using Core.UI;

namespace Runtime.Game
{
    public class MenuStateController : StateController
    {
        private readonly IUiService _uiService;

        private MenuScreen _screen;
        private MenuPopup _popup;

        public MenuStateController(ILogger logger, IUiService uiService) : base(logger)
        {
            _uiService = uiService;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            CreateScreen();
            SubscribeToEvents();
            return UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            if (_popup)
                _popup.DestroyPopup();

            UnsubscribeToEvents();
            await _uiService.HideScreen(ConstScreens.MenuScreen);
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<MenuScreen>(ConstScreens.MenuScreen);
            _screen.Initialize();
            _screen.ShowAsync().Forget();
        }

        private void SubscribeToEvents()
        {
            _screen.OnPlayPressed += async () => await GoTo<LevelSelectionStateController>();
            _screen.OnHowToPlayPressed += ShowHowToPlayPopup;
            _screen.OnPrivacyPressed += ShowPrivacyPolicyPopup;
            _screen.OnSettingsPressed += async () => await GoTo<SettingsStateController>();
            _screen.OnShopPressed += async () => await GoTo<ShopStateController>();
            _screen.OnAccountPressed += async () => await GoTo<AccountStateController>();
            _screen.OnLeaderboardPressed += async () => await GoTo<LeaderboardStateController>();
        }

        private void UnsubscribeToEvents()
        {
            _screen.OnHowToPlayPressed -= ShowHowToPlayPopup;
            _screen.OnPrivacyPressed -= ShowPrivacyPolicyPopup;
        }

        private void ShowHowToPlayPopup()
        {
            _uiService.ShowPopup(ConstPopups.RulesPopup);
        }
        private void ShowPrivacyPolicyPopup()
        {
            _uiService.ShowPopup(ConstPopups.PrivacyPolicyPopup);
        }
        private void ShowTermsPopup()
        {
            _uiService.ShowPopup(ConstPopups.TermsPopup);
        }
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using Runtime.Game;
using Core.StateMachine;
using ILogger = Core.ILogger;

namespace Runtime.GameStateMachine
{
    public class GameState : StateController
    {
        private readonly StateMachine _stateMachine;

        private readonly MenuStateController _menuStateController;
        private readonly LevelSelectionStateController _levelSelectionController;
        private readonly ShopStateController _shopStateController;
        private readonly AccountStateController _accountStateController;
        private readonly GameplayStateController _gameplayStateController;
        private readonly LeaderboardStateController _leaderboardStateController;
        private readonly SettingsStateController _settingsStateController;
        private readonly UserDataStateChangeController _userDataStateChangeController;

        public GameState(ILogger logger,
            MenuStateController menuStateController,
            LevelSelectionStateController levelSelectionController,
            ShopStateController shopStateController,
            AccountStateController accountStateController,
            GameplayStateController gameplayStateController,
            LeaderboardStateController leaderboardStateController,
            SettingsStateController settingsStateController,
            StateMachine stateMachine,
            UserDataStateChangeController userDataStateChangeController) : base(logger)
        {
            _stateMachine = stateMachine;
            _menuStateController = menuStateController;
            _levelSelectionController = levelSelectionController;
            _shopStateController = shopStateController;
            _accountStateController = accountStateController;
            _gameplayStateController = gameplayStateController;
            _leaderboardStateController = leaderboardStateController;
            _settingsStateController = settingsStateController;
            _userDataStateChangeController = userDataStateChangeController;
        }

        public override async UniTask Enter(CancellationToken cancellationToken)
        {
            await _userDataStateChangeController.Run(default);

            _stateMachine.Initialize(_menuStateController, _levelSelectionController, _shopStateController, _accountStateController, _gameplayStateController, _leaderboardStateController, _settingsStateController);
            _stateMachine.GoTo<MenuStateController>().Forget();
        }
    }
}
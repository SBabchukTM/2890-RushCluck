using System.Threading;
using Core.StateMachine;
using Runtime.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;
using Runtime.Services.UserData;
using Core.Services.Audio;
using Runtime.Services.Audio;

namespace Runtime.Game
{
    public class SettingsStateController : StateController
    {
        private readonly IUiService _uiService;
        private readonly IAudioService _audioService;
        private readonly UserDataService _userDataService;

        private SettingsScreen _screen;

        public SettingsStateController(ILogger logger, IUiService uiService, IAudioService audioService, UserDataService userDataService) : base(logger)
        {
            _uiService = uiService;
            _audioService = audioService;
            _userDataService = userDataService;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            CreateScreen();
            SubscribeToEvents();
            return UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            _screen.OnMusicChanged -= OnMusicChanged;
            _screen.OnSoundChanged -= OnSoundChanged;
            _screen.OnVibrationChanged -= OnVibrationChanged;

            await _uiService.HideScreen(ConstScreens.SettingsScreen);
        }

        private void CreateScreen()
        {
            _screen = _uiService.GetScreen<SettingsScreen>(ConstScreens.SettingsScreen);
            _screen.Initialize(_userDataService.GetUserData().SettingsData);
            _screen.ShowAsync().Forget();
        }

        private void SubscribeToEvents()
        {
            _screen.OnBackPressed += async () => await GoTo<MenuStateController>();

            _screen.OnMusicChanged += OnMusicChanged;
            _screen.OnSoundChanged += OnSoundChanged;
            _screen.OnVibrationChanged += OnVibrationChanged;
        }

        private void OnVibrationChanged(bool state)
        {
            var userData = _userDataService.GetUserData();
            userData.SettingsData.IsVibration = state;

            if (state)
                UnityEngine.Handheld.Vibrate();

            _audioService.PlaySound(ConstAudio.PressButtonSound);
        }

        private void OnSoundChanged(bool state)
        {
            _audioService.SetVolume(AudioType.Sound, state ? 1 : 0);
            var userData = _userDataService.GetUserData();
            userData.SettingsData.IsSoundVolume = state;

            _audioService.PlaySound(ConstAudio.PressButtonSound);
        }

        private void OnMusicChanged(bool state)
        {
            _audioService.SetVolume(AudioType.Music, state ? 1 : 0);
            var userData = _userDataService.GetUserData();
            userData.SettingsData.IsMusicVolume = state;

            _audioService.PlaySound(ConstAudio.PressButtonSound);
        }
    }
}
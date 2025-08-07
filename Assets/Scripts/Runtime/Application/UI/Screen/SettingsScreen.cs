using Runtime.Services.UserData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class SettingsScreen : UiScreen
    {
        [SerializeField] private SimpleButton _backButton;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _soundToggle;
        [SerializeField] private Toggle _vibrationToggle;

        public event Action OnBackPressed;

        public event Action<bool> OnMusicChanged;
        public event Action<bool> OnSoundChanged;
        public event Action<bool> OnVibrationChanged;

        private void OnDestroy()
        {
            _backButton.Button.onClick.RemoveAllListeners();
        }

        public void Initialize(SettingsData settingsData)
        {
            SubscribeToEvents();
            SetData(settingsData);
        }

        private void SetData(SettingsData settingsData)
        {
            _musicToggle.isOn = settingsData.IsMusicVolume;
            _soundToggle.isOn = settingsData.IsSoundVolume;
            _vibrationToggle.isOn = settingsData.IsVibration;
        }

        private void SubscribeToEvents()
        {
            _backButton.Button.onClick.AddListener(() => OnBackPressed?.Invoke());

            _musicToggle.onValueChanged.AddListener((value) => OnMusicChanged?.Invoke(value));
            _soundToggle.onValueChanged.AddListener((value) => OnSoundChanged?.Invoke(value));
            _vibrationToggle.onValueChanged.AddListener((value) => OnVibrationChanged?.Invoke(value));
        }
    }
}
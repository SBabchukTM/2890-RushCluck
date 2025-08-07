using System;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class MenuScreen : UiScreen
    {
        [SerializeField] private SimpleButton _htpButton;
        [SerializeField] private SimpleButton _privacyButton;
        [SerializeField] private SimpleButton _playButton;
        [SerializeField] private SimpleButton _accountButton;
        [SerializeField] private SimpleButton _shopButton;
        [SerializeField] private SimpleButton _settingsButton;
        [SerializeField] private SimpleButton _leaderboardButton;

        public event Action OnHowToPlayPressed;
        public event Action OnPlayPressed;
        public event Action OnAccountPressed;
        public event Action OnSettingsPressed;
        public event Action OnLeaderboardPressed;
        public event Action OnShopPressed;
        public event Action OnPrivacyPressed;

        private void OnDestroy()
        {
            _htpButton.Button.onClick.RemoveAllListeners();
            _privacyButton.Button.onClick.RemoveAllListeners();
            _playButton.Button.onClick.RemoveAllListeners();
            _accountButton.Button.onClick.RemoveAllListeners();
            _shopButton.Button.onClick.RemoveAllListeners();
            _settingsButton.Button.onClick.RemoveAllListeners();
            _leaderboardButton.Button.onClick.RemoveAllListeners();
        }

        public void Initialize()
        {
            _htpButton.Button.onClick.AddListener(() => OnHowToPlayPressed?.Invoke());
            _playButton.Button.onClick.AddListener(() => OnPlayPressed?.Invoke());
            _privacyButton.Button.onClick.AddListener(() => OnPrivacyPressed?.Invoke());
            _playButton.Button.onClick.AddListener(() => OnPlayPressed?.Invoke());
            _accountButton.Button.onClick.AddListener(() => OnAccountPressed?.Invoke());
            _shopButton.Button.onClick.AddListener(() => OnShopPressed?.Invoke());
            _settingsButton.Button.onClick.AddListener(() => OnSettingsPressed?.Invoke());
            _leaderboardButton.Button.onClick.AddListener(() => OnLeaderboardPressed?.Invoke());
        }
    }
}
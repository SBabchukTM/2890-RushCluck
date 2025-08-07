using Runtime.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class MenuPopup : BasePopup
    {
        [SerializeField] private SimpleButton _closeButton;
        [SerializeField] private SimpleButton _accButton;
        [SerializeField] private SimpleButton _setButton;
        [SerializeField] private SimpleButton _leadButton;
        [SerializeField] private SimpleButton _shopButton;
        [SerializeField] private SimpleButton _termsButton;
        [SerializeField] private SimpleButton _privacyButton;

        public event Action OnAccountPressed;
        public event Action OnSettingsPressed;
        public event Action OnLeaderboardPressed;
        public event Action OnShopPressed;
        public event Action OnTermsPressed;
        public event Action OnPrivacyPressed;

        private void OnDestroy()
        {
            _closeButton.Button.onClick.RemoveAllListeners();

            _accButton.Button.onClick.RemoveAllListeners();
            _setButton.Button.onClick.RemoveAllListeners();
            _leadButton.Button.onClick.RemoveAllListeners();
            _shopButton.Button.onClick.RemoveAllListeners();
            _termsButton.Button.onClick.RemoveAllListeners();
            _privacyButton.Button.onClick.RemoveAllListeners();
        }

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _closeButton.Button.onClick.AddListener(DestroyPopup);

            _accButton.Button.onClick.AddListener(() => OnAccountPressed?.Invoke());
            _setButton.Button.onClick.AddListener(() => OnSettingsPressed?.Invoke());
            _leadButton.Button.onClick.AddListener(() => OnLeaderboardPressed?.Invoke());
            _shopButton.Button.onClick.AddListener(() => OnShopPressed?.Invoke());
            _termsButton.Button.onClick.AddListener(() => OnTermsPressed?.Invoke());
            _privacyButton.Button.onClick.AddListener(() => OnPrivacyPressed?.Invoke());
            return base.Show(data, cancellationToken);
        }
    }
}
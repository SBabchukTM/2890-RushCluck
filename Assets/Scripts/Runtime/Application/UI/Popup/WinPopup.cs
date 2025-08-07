using Runtime.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class WinPopup : BasePopup
    {
        [SerializeField] private SimpleButton _homeButton;
        [SerializeField] private SimpleButton _restartButton;
        [SerializeField] private SimpleButton _continueButton;
        [SerializeField] private TextMeshProUGUI _collectedText;

        public event Action OnHomePressed;
        public event Action OnRestartPressed;
        public event Action OnContinuePressed;

        private void OnDestroy()
        {
            _homeButton.Button.onClick.RemoveAllListeners();
            _restartButton.Button.onClick.RemoveAllListeners();
            _continueButton.Button.onClick.RemoveAllListeners();
        }

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _homeButton.Button.onClick.AddListener(() => OnHomePressed?.Invoke());
            _restartButton.Button.onClick.AddListener(() => OnRestartPressed?.Invoke());
            _continueButton.Button.onClick.AddListener(() => OnContinuePressed?.Invoke());
            return base.Show(data, cancellationToken);
        }

        public void SetData(int collected)
        {
            _collectedText.text = "COLLECTED: " + collected;
        }
    }
}
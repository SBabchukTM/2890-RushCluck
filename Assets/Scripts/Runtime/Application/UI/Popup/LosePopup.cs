using Runtime.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class LosePopup : BasePopup
    {
        [SerializeField] private SimpleButton _homeButton;
        [SerializeField] private SimpleButton _restartButton;
        [SerializeField] private TextMeshProUGUI _distanceText;

        public event Action OnHomePressed;
        public event Action OnRestartPressed;

        private void OnDestroy()
        {
            _homeButton.Button.onClick.RemoveAllListeners();
            _restartButton.Button.onClick.RemoveAllListeners();
        }

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _homeButton.Button.onClick.AddListener(() => OnHomePressed?.Invoke());
            _restartButton.Button.onClick.AddListener(() => OnRestartPressed?.Invoke());
            return base.Show(data, cancellationToken);
        }

        public void SetData(int distance)
        {
            _distanceText.text = "DISTANCE : " + distance;
        }
    }
}
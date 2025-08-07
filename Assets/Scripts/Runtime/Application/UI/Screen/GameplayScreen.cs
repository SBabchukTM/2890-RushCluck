using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class GameplayScreen : UiScreen
    {
        [SerializeField] private SimpleButton _pauseButton;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private TextMeshProUGUI _collectiblesText;

        public event Action OnPausePressed;

        private void OnDestroy()
        {
            _pauseButton.Button.onClick.RemoveAllListeners();
        }

        public void Initialize()
        {
            _pauseButton.Button.onClick.AddListener(() => OnPausePressed?.Invoke());
        }

        public void UpdateProgress(int progress, int target) => _progressText.text = $"{progress} / {target}M";
        public void UpdateCollectibles(int collected) => _collectiblesText.text = $"+{collected}";
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class LeaderboardScreen : UiScreen
    {
        [SerializeField] private SimpleButton _backButton;
        [SerializeField] private RectTransform _parent;

        public event Action OnBackPressed;

        private void OnDestroy()
        {
            _backButton.Button.onClick.RemoveAllListeners();
        }

        public void Initialize(List<RecordDisplay> recordsList)
        {
            _backButton.Button.onClick.AddListener(() => OnBackPressed?.Invoke());

            foreach (var record in recordsList)
                record.transform.SetParent(_parent, false);
        }
    }
}
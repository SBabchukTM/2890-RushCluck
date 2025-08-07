using System;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class LevelSelectionScreen : UiScreen
    {
        [SerializeField] private SimpleButton _backButton;
        [SerializeField] private SimpleButton _playButton;

        [SerializeField, Space] private LevelSelectionButton[] _levelSelectionButtons;

        [SerializeField, Space] private LevelSelectionButtonStatusDisplay _selectedButtonDisplay;
        [SerializeField] private LevelSelectionButtonStatusDisplay _notSelectedButtonDisplay;
        [SerializeField] private LevelSelectionButtonStatusDisplay _lockedButtonDisplay;

        public event Action OnBackPressed;
        public event Action OnPlayPressed;
        public event Action<int> OnSelectedLevelChanged;

        private int _lastSelectedButtonID = 0;

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        public void Initialize(int lastUnlockedID)
        {
            SubscribeToEvents();
            InitializeButtons(lastUnlockedID);
        }

        private void SubscribeToEvents()
        {
            _backButton.Button.onClick.AddListener(() => OnBackPressed?.Invoke());
            _playButton.Button.onClick.AddListener(() => OnPlayPressed?.Invoke());
        }

        private void UnsubscribeFromEvents()
        {
            _backButton.Button.onClick.RemoveAllListeners();
            _playButton.Button.onClick.RemoveAllListeners();

            int size = _levelSelectionButtons.Length;
            for (int i = 0; i < size; i++)
                _levelSelectionButtons[i].OnLevelSelected -= UpdateSelectedLevel;
        }

        private void InitializeButtons(int lastUnlockedLevelID)
        {
            int size = _levelSelectionButtons.Length;

            _lastSelectedButtonID = lastUnlockedLevelID;

            for (int i = 0; i < size; i++)
            {
                bool locked = i > lastUnlockedLevelID;
                bool selected = i == lastUnlockedLevelID;

                var button = _levelSelectionButtons[i];
                InitializeButton(locked, selected, button);
                button.OnLevelSelected += UpdateSelectedLevel;
            }
        }

        private void InitializeButton(bool locked, bool selected, LevelSelectionButton button)
        {
            button.Initialize(locked);
            if (selected)
                SetButtonStatusDisplay(button, _selectedButtonDisplay);
            else if (locked)
                SetButtonStatusDisplay(button, _lockedButtonDisplay);
            else
                SetButtonStatusDisplay(button, _notSelectedButtonDisplay);
        }

        private void SetButtonStatusDisplay(LevelSelectionButton button, LevelSelectionButtonStatusDisplay display)
        {
            button.SetColor(display.Color);
            if (display.Sprite)
                button.SetSprite(display.Sprite);
        }

        private void UpdateSelectedLevel(int level)
        {
            SetButtonStatusDisplay(_levelSelectionButtons[_lastSelectedButtonID], _notSelectedButtonDisplay);
            OnSelectedLevelChanged?.Invoke(level);

            _lastSelectedButtonID = level;
            SetButtonStatusDisplay(_levelSelectionButtons[_lastSelectedButtonID], _selectedButtonDisplay);
        }
    }

    [Serializable]
    public class LevelSelectionButtonStatusDisplay
    {
        [Header("If Sprite is Null, it won't be set")]
        public Sprite Sprite;
        public Color Color = Color.white;
    }
}
using Runtime.Application.UserAccountSystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class AccountScreen : UiScreen
    {
        [SerializeField] private SimpleButton _backButton;
        [SerializeField] private SimpleButton _saveButton;
        [SerializeField] private SimpleButton _changeAvatarButton;
        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] private TMP_InputField _ageField;
        [SerializeField] private TMP_InputField _genderField;
        [SerializeField] private Image _avatarImage;

        public event Action OnBackPressed;
        public event Action OnSavePressed;
        public event Action OnChangeAvatarPressed;

        public event Action<string> OnNameChanged;
        public event Action<string> OnAgeChanged;
        public event Action<string> OnGenderChanged;

        private void OnDestroy()
        {
            _backButton.Button.onClick.RemoveAllListeners();
            _saveButton.Button.onClick.RemoveAllListeners();

            _nameField.onEndEdit.RemoveAllListeners();
            _ageField.onEndEdit.RemoveAllListeners();
            _genderField.onEndEdit.RemoveAllListeners();
        }

        public void Initialize()
        {
            SubscribeToEvents();
        }

        public void SetData(UserAccountData data)
        {
            _nameField.text = data.Username;
            _ageField.text = data.Age.ToString();
            _genderField.text = data.Gender;
        }

        public void SetAvatar(Sprite sprite)
        {
            if (sprite != null)
                _avatarImage.sprite = sprite;
        }

        private void SubscribeToEvents()
        {
            _backButton.Button.onClick.AddListener(() => OnBackPressed?.Invoke());
            _saveButton.Button.onClick.AddListener(() => OnSavePressed?.Invoke());
            _changeAvatarButton.Button.onClick.AddListener(() => OnChangeAvatarPressed?.Invoke());

            _nameField.onEndEdit.AddListener((value) => OnNameChanged?.Invoke(value));
            _ageField.onEndEdit.AddListener((value) => OnAgeChanged?.Invoke(value));
            _genderField.onEndEdit.AddListener((value) => OnGenderChanged?.Invoke(value));
        }
    }
}
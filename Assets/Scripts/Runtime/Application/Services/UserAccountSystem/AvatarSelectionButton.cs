using System;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Application.UserAccountSystem
{
    public class AvatarSelectionButton : MonoBehaviour
    {
        [SerializeField] private Image _avatarImage;
        [SerializeField] private Button _button;

        public event Action<Sprite> OnAvatarSelected;
    
        public void Initialize(Sprite avatar)
        {
            _avatarImage.sprite = avatar;   
            _button.onClick.AddListener(() => OnAvatarSelected?.Invoke(avatar));
        }
    }   
}

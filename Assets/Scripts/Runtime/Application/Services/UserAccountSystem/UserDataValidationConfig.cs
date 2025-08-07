using Core;
using UnityEngine;

namespace Runtime.Application.UserAccountSystem
{
    [CreateAssetMenu(fileName = "UserDataValidationConfig", menuName = "Config/UserDataValidationConfig")]
    public class UserDataValidationConfig : BaseSettings
    {
        [SerializeField, Tooltip("2-14 symbols, starts with a letter")] 
        private string _usernameRegex = "^[A-Za-z][A-Za-z0-9]{1,13}$";
    
        [SerializeField, Tooltip("2-14 letters")] 
        private string _genderRegex = "^[A-Za-z]{2,14}$";
    
        [SerializeField] private int _minAge = 18;
        [SerializeField] private int _maxAge = 99;
    
        public string UsernameRegex => _usernameRegex;
        public string GenderRegex => _genderRegex;
        public int MinAge => _minAge;
        public int MaxAge => _maxAge;
    }
}

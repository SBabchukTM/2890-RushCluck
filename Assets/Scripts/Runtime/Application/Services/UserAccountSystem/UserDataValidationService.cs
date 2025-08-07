using System.Text.RegularExpressions;
using Core;

namespace Runtime.Application.UserAccountSystem
{
    public class UserDataValidationService
    {
        private readonly ISettingProvider _settingProvider;

        public UserDataValidationService(ISettingProvider settingProvider)
        {
            _settingProvider = settingProvider;
        }

        public bool IsNameValid(string name)
        {
            UserDataValidationConfig userDataValidationConfig = _settingProvider.Get<UserDataValidationConfig>();
            return !string.IsNullOrWhiteSpace(name) && 
                   Regex.IsMatch(name, userDataValidationConfig.UsernameRegex);
        }
        
        public bool IsAgeValid(string ageText)
        {
            if (!int.TryParse(ageText, out int age))
                return false;
            
            UserDataValidationConfig userDataValidationConfig = _settingProvider.Get<UserDataValidationConfig>();
            return age >= userDataValidationConfig.MinAge && 
                   age <= userDataValidationConfig.MaxAge;
        }
        
        public bool IsGenderValid(string gender)
        {
            UserDataValidationConfig userDataValidationConfig = _settingProvider.Get<UserDataValidationConfig>();
            return !string.IsNullOrWhiteSpace(gender) &&  
                   Regex.IsMatch(gender, userDataValidationConfig.GenderRegex);
        }
    }   
}

using System;

namespace Runtime.Application.UserAccountSystem
{
    [Serializable]
    public class UserAccountData
    {
        public string Username = "Player";
        public int Age = 18;
        public string Gender = "male";
        public string AvatarBase64 = String.Empty;

        public UserAccountData Copy()
        {
            return (UserAccountData)MemberwiseClone();
        }
    }
}
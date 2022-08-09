using System;
using Xamarin.Essentials;

namespace Next2.Services.Settings
{
    public class SettingsManager : ISettingsManager
    {
        #region -- ISettingsManager implementation --

        public int UserId
        {
            get => Preferences.Get(nameof(UserId), -1);
            set => Preferences.Set(nameof(UserId), value);
        }

        public bool IsAuthorizationComplete
        {
            get => Preferences.Get(nameof(IsAuthorizationComplete), false);
            set => Preferences.Set(nameof(IsAuthorizationComplete), value);
        }

        public bool IsUserAdmin
        {
            get => Preferences.Get(nameof(IsUserAdmin), false);
            set => Preferences.Set(nameof(IsUserAdmin), value);
        }

        public string Token
        {
            get => Preferences.Get(nameof(Token), string.Empty);
            set => Preferences.Set(nameof(Token), value);
        }

        public string RefreshToken
        {
            get => Preferences.Get(nameof(RefreshToken), string.Empty);
            set => Preferences.Set(nameof(RefreshToken), value);
        }

        public DateTime TokenExpirationDate
        {
            get => Preferences.Get(nameof(TokenExpirationDate), DateTime.Now);
            set => Preferences.Set(nameof(TokenExpirationDate), value);
        }

        public string LastCurrentOrderIds
        {
            get => Preferences.Get(nameof(LastCurrentOrderIds), string.Empty);
            set => Preferences.Set(nameof(LastCurrentOrderIds), value);
        }

        public void Clear()
        {
            Preferences.Clear();
        }

        #endregion
    }
}

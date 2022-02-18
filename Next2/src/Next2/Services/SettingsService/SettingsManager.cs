using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Next2.Services.SettingsService
{
    public class SettingsManager : ISettingsManager
    {
        public int UserId
        {
            get => Preferences.Get(nameof(UserId), -1);
            set => Preferences.Set(nameof(UserId), value);
        }

        public string UserName
        {
            get => Preferences.Get(nameof(UserName), null);
            set => Preferences.Set(nameof(UserName), value);
        }
    }
}

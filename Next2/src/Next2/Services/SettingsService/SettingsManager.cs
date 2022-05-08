﻿using Next2.Interfaces;
using System;
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

        public bool AuthComplete
        {
            get => Preferences.Get(nameof(AuthComplete), false);
            set => Preferences.Set(nameof(AuthComplete), value);
        }

        public string? Token
        {
            get => Preferences.Get(nameof(Token), string.Empty);
            set => Preferences.Set(nameof(Token), value);
        }

        public string? RefreshToken
        {
            get => Preferences.Get(nameof(RefreshToken), string.Empty);
            set => Preferences.Set(nameof(RefreshToken), value);
        }

        public DateTime TokenExpirationDate
        {
            get => Preferences.Get(nameof(TokenExpirationDate), DateTime.Now);
            set => Preferences.Set(nameof(TokenExpirationDate), value);
        }
    }
}

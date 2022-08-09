using System;

namespace Next2.Services.Settings
{
    public interface ISettingsManager
    {
        int UserId { get; set; }

        bool IsAuthorizationComplete { get; set; }

        bool IsUserAdmin { get; set; }

        string Token { get; set; }

        string RefreshToken { get; set; }

        DateTime TokenExpirationDate { get; set; }

        string LastCurrentOrderIds { get; set; }

        void Clear();
    }
}

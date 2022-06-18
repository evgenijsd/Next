using System;

namespace Next2.Services.SettingsService
{
    public interface ISettingsManager
    {
        int UserId { get; set; }

        bool IsAuthorizationComplete { get; set; }

        string? Token { get; set; }

        string? RefreshToken { get; set; }

        DateTime TokenExpirationDate { get; set; }

        string? LastCurrentOrderIds { get; set; }
    }
}

using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Services.UserService;
using Next2.Services.SettingsService;
using System;
using System.Threading.Tasks;

namespace Next2.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IUserService _userService;

        public AuthenticationService(
            IUserService userService,
            ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _userService = userService;
        }

        #region -- Public properties --

        public int AuthorizedUserId { get => _settingsManager.UserId; }

        #endregion

        #region -- AuthenticationService implementation --

        public void Authorization()
        {
            if (_userService?.AuthorizedUserModel is not null)
            {
                _settingsManager.UserId = _userService.AuthorizedUserModel.Id;
                _settingsManager.UserName = _userService.AuthorizedUserModel.UserName;
            }
        }

        public void LogOut()
        {
            _settingsManager.UserId = -1;
            _settingsManager.UserName = default;
        }

        #endregion
    }
}

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

        public UserModel User { get; private set; } // named user

        #endregion

        #region -- AuthenticationService implementation --

        public async Task<AOResult<UserModel>> AuthorizeAsync(int userId)
        {
            var result = new AOResult<UserModel>();

            try
            {
                var user = await _userService.CheckUserExists(userId);

                if (user.IsSuccess)
                {
                    User = user.Result;
                    _settingsManager.UserId = user.Result.Id;
                    _settingsManager.UserName = user.Result.UserName;

                    result.SetSuccess();
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AuthorizeAsync)}: exception", "Error from UserService AuthorizationAsync", ex);
            }

            return result;
        }

        public void LogOut()
        {
            _settingsManager.UserId = default;
            _settingsManager.UserName = default;
        }

        #endregion
    }
}

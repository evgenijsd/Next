using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Services.ProfileService;
using Next2.Services.Services;
using System;
using System.Threading.Tasks;

namespace Next2.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private ISettingsManager _settingsManager;
        private IUserService _userService;

        public AuthenticationService(
            IUserService userService,
            ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
            _userService = userService;
        }

        #region -- Public properties --

        private UserModel _user;
        public UserModel User { get => _user; }

        #endregion

        #region -- AuthenticationService implementation --

        public async Task<AOResult<UserModel>> AuthorizationAsync(int userId)
        {
            var result = new AOResult<UserModel>();

            try
            {
                var user = await _userService.CheckUserExists(userId);

                if (user.IsSuccess)
                {
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
                result.SetError($"{nameof(AuthorizationAsync)}: exception", "Error from UserService AuthorizationAsync", ex);
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

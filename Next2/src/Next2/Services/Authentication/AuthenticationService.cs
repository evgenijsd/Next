using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Services.Mock;
using Next2.Services.Rest;
using Next2.Services.SettingsService;
using System;
using System.Threading.Tasks;

namespace Next2.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMockService _mockService;
        private readonly IRestService _restService;
        private readonly ISettingsManager _settingsManager;

        public AuthenticationService(
            IMockService mockService,
            ISettingsManager settingsManager,
            IRestService restService)
        {
            _mockService = mockService;
            _restService = restService;
            _settingsManager = settingsManager;
        }

        #region -- Public properties --

        public int AuthorizedUserId { get => _settingsManager.UserId; }

        public bool IsAuthorizationComplete { get => _settingsManager.IsAuthorizationComplete; }

        public string? Token { get => _settingsManager.Token; }

        public string? RefreshToken { get => _settingsManager.RefreshToken; }

        #endregion

        #region -- AuthenticationService implementation --

        public async Task<AOResult<UserModel?>> CheckUserExists(int userId)
        {
            var result = new AOResult<UserModel?>();

            try
            {
                var user = await _mockService.GetByIdAsync<UserModel>(userId);

                if (user != null)
                {
                    result.SetSuccess(user);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(CheckUserExists)}: exception", "Error from UserService CheckUserExists", ex);
            }

            return result;
        }

        public void LogOut()
        {
            _settingsManager.UserId = -1;
            _settingsManager.IsAuthorizationComplete = false;
            _settingsManager.Token = string.Empty;
            _settingsManager.RefreshToken = string.Empty;
            _settingsManager.TokenExpirationDate = DateTime.Now;
        }

        public async Task<AOResult> AuthorizationUserAsync(string userId)
        {
            var result = new AOResult();
            result.SetSuccess();

            _settingsManager.UserId = 0;

            return result;
        }

        public async Task<AOResult> LogoutAsync()
        {
            var result = new AOResult();

            return result;
        }

        #endregion
    }
}

﻿using Next2.Helpers.DTO;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using Next2.Services.Rest;
using Next2.Services.SettingsService;
using System;
using System.Net.Http;
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

        public int AuthorizedUserId => _settingsManager.UserId;

        public bool IsAuthorizationComplete => _settingsManager.IsAuthorizationComplete;

        #endregion

        #region -- AuthenticationService implementation --

        public async Task<AOResult<UserModel?>> CheckUserExists(int userId)
        {
            var result = new AOResult<UserModel?>();

            try
            {
                var user = await _mockService.GetByIdAsync<UserModel>(userId);

                if (user is not null)
                {
                    result.SetSuccess(user);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(CheckUserExists)}: exception", "Error from UserService CheckUserExists", ex);
            }

            return result;
        }

        public async Task<AOResult> AuthorizeUserAsync(string userId)
        {
            var result = new AOResult();

            var employee = new LoginQuery()
            {
                EmployeeId = userId,
            };

            try
            {
                if (int.TryParse(userId, out int id))
                {
                    var response = await _restService.RequestAsync<GenericExecutionResult<LoginQueryResult>>(HttpMethod.Post, $"{Constants.API.HOST_URL}/api/auth/login", employee);

                    if (response.Success)
                    {
                        _settingsManager.UserId = id;
                        _settingsManager.IsAuthorizationComplete = true;
                        _settingsManager.Token = response.Value.Tokens.AccessToken;
                        _settingsManager.RefreshToken = response.Value.Tokens.RefreshToken;
                        _settingsManager.TokenExpirationDate = DateTime.Now.AddHours(Constants.API.TOKEN_EXPIRATION_TIME);

                        result.SetSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AuthorizeUserAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> LogoutAsync()
        {
            var result = new AOResult();

            var employee = new LogoutCommand()
            {
                EmployeeId = _settingsManager.UserId.ToString(),
                RefreshToken = _settingsManager.RefreshToken,
            };

            try
            {
                await _restService.RequestAsync<ExecutionResult>(HttpMethod.Post, $"{Constants.API.HOST_URL}/api/auth/logout", employee);

                _settingsManager.UserId = -1;
                _settingsManager.IsAuthorizationComplete = false;
                _settingsManager.Token = string.Empty;
                _settingsManager.RefreshToken = string.Empty;
                _settingsManager.TokenExpirationDate = DateTime.Now;

                result.SetSuccess();
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(LogoutAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}

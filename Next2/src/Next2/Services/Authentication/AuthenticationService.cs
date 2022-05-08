﻿using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Services.UserService;
using Next2.Services.SettingsService;
using System;
using System.Threading.Tasks;
using Next2.Services.Mock;
using Next2.Enums;
using Next2.Services.Rest;

namespace Next2.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMockService _mockService;
        private readonly IRestService _restService;
        private readonly ISettingsManager _settingsManager;
        private int currentUser;

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
                    currentUser = user.Id;
                }
                else
                {
                    result.SetFailure();
                    currentUser = -1;
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(CheckUserExists)}: exception", "Error from UserService CheckUserExists", ex);
            }

            return result;
        }

        public void Authorization()
        {
            if (currentUser >= 0)
            {
                _settingsManager.UserId = currentUser;
            }
        }

        public void LogOut()
        {
            _settingsManager.UserId = -1;
        }

        public async Task<AOResult> LoginAsync()
        {
            var result = new AOResult();

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

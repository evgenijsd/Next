﻿using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Services.MockService;
using Next2.Services.SettingsService;
using System;
using System.Threading.Tasks;

namespace Next2.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IMockService _mockService;

        private ISettingsManager _settingsManager;

        public UserService(
            ISettingsManager settingsManager,
            IMockService mockService)
        {
            _mockService = mockService;
            _settingsManager = settingsManager;
        }

        #region -- UserService implementation  --

        public async Task<AOResult<int>> AddUserAsync(UserModel user)
        {
            var result = new AOResult<int>();

            try
            {
                int userId = await _mockService.AddAsync(user);

                if (userId > 0)
                {
                    result.SetSuccess();
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddUserAsync)}: exception", "Exeption from UserService AddUserAsync", ex);
            }

            return result;
        }

        public async Task<AOResult<UserModel>> GetUserById(int userId)
        {
            var result = new AOResult<UserModel>();

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
                result.SetError($"{nameof(GetUserById)}: exception", "Error from UserService GetUserById", ex);
            }

            return result;
        }

        #endregion

    }
}
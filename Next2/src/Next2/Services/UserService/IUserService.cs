﻿using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Threading.Tasks;

namespace Next2.Services.ProfileService
{
    public interface IUserService
    {
        Task<AOResult<UserModel>> CheckUserExists(int userId);
        Task<AOResult<int>> AddUserAsync(UserModel user);
    }
}

﻿using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Threading.Tasks;

namespace Next2.Services.UserService
{
    public interface IUserService
    {
        Task<AOResult<UserModel>> GetUserById(int userId);
        Task<AOResult<int>> AddUserAsync(UserModel user);
    }
}

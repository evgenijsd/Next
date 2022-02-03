using Next2.Helpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.ProfileService
{
    public interface IUserService
    {
        Task<AOResult<UserModel>> CheckUserExists(int userId);
        Task<AOResult<int>> AddUserAsync(UserModel user);
    }
}

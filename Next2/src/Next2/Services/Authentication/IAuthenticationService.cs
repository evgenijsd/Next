using Next2.Helpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Authentication
{
    public interface IAuthenticationService
    {
        UserModel User { get; }
        Task<AOResult<UserModel>> AuthorizationAsync(int userId);
        void LogOut();
    }
}

using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Threading.Tasks;

namespace Next2.Services.Authentication
{
    public interface IAuthenticationService
    {
        int AuthorizedUserId { get; }

        bool IsAuthorizationComplete { get; }

        Task<AOResult<UserModel?>> CheckUserExists(int userId);

        Task<AOResult> AuthorizeUserAsync(string userId);

        Task<AOResult> LogoutAsync();
    }
}

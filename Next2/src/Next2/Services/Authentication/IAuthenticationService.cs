using Next2.Helpers.ProcessHelpers;
using Next2.Models.API.Results;
using System.Threading.Tasks;

namespace Next2.Services.Authentication
{
    public interface IAuthenticationService
    {
        int AuthorizedUserId { get; }

        bool IsAuthorizationComplete { get; }

        bool IsUserAdmin { get; }

        Task<AOResult<LoginQueryResult>> GetUserById(string userId);

        Task<AOResult> AuthorizeUserAsync(string userId);

        Task<AOResult> LogoutAsync();
    }
}

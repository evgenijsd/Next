using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Threading.Tasks;

namespace Next2.Services.Authentication
{
    public interface IAuthenticationService
    {
        int AuthorizedUserId { get; }

        Task<AOResult<UserModel?>> CheckUserExists(int userId);

        void Authorization();

        void LogOut();

        Task<AOResult> LoginAsync();

        Task<AOResult> LogoutAsync();
    }
}

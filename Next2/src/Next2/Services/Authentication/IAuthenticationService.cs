using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Threading.Tasks;

namespace Next2.Services.Authentication
{
    public interface IAuthenticationService
    {
        int AuthorizedUserId { get; }
        Task<AOResult> CheckUserExists(int userId);
        void Authorization();
        void LogOut();
    }
}

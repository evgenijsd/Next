using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Threading.Tasks;

namespace Next2.Services.Authentication
{
    public interface IAuthenticationService
    {
        int AuthorizedUserId { get; }
        void Authorization();
        void LogOut();
    }
}

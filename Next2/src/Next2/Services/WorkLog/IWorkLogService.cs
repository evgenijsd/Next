using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.Results;
using System.Threading.Tasks;

namespace Next2.Services.WorkLog
{
    public interface IWorkLogService
    {
        Task<AOResult<TimeTrackResult>> LogWorkTimeAsync(string employeeId);
    }
}

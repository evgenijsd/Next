using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Threading.Tasks;

namespace Next2.Services.WorkLog
{
    public interface IWorkLogService
    {
        Task<AOResult<EEmployeeRegisterState>> LogWorkTimeAsync(WorkLogRecordModel record);
    }
}

using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.Log
{
    public interface ILogService
    {
        Task<AOResult<EEmployeeRegisterState>> InsertRecord(WorkLogRecordModel record);
    }
}

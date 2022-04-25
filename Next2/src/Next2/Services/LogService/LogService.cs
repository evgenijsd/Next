using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Services.Authentication;
using Next2.Services.Mock;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.Log
{
    public class LogService : ILogService
    {
        private readonly IMockService _mockService;
        private readonly IAuthenticationService _authenticationService;
        public LogService(
            IMockService mockService,
            IAuthenticationService authenticationService)
        {
            _mockService = mockService;
            _authenticationService = authenticationService;
        }

        public async Task<AOResult<EEmployeeRegisterState>> InsertRecord(WorkLogRecordModel record)
        {
            var result = new AOResult<EEmployeeRegisterState>();
            var user = await _authenticationService.CheckUserExists(record.EmployeeId);

            if (user.IsSuccess)
            {
                try
                {
                    var records = await _mockService.GetAllAsync<WorkLogRecordModel>();
                    if (records == null || records.Count() == 0)
                    {
                        record.State = EEmployeeRegisterState.CheckedIn;
                        int id = await _mockService.AddAsync(record);
                        if (id == record.EmployeeId)
                        {
                            result.SetSuccess(record.State);
                        }
                        else
                        {
                            result.SetFailure();
                        }
                    }
                    else
                    {
                        var lastRecord = records.OrderBy(x => x.Id).Where(y => y.EmployeeId == record.EmployeeId).LastOrDefault();
                        if (lastRecord == null)
                        {
                            record.State = EEmployeeRegisterState.CheckedIn;
                            var id = await _mockService.AddAsync(record);
                            if (id > 0)
                            {
                                result.SetSuccess(record.State);
                            }
                            else
                            {
                                result.SetFailure();
                            }
                        }
                        else if (lastRecord.State == EEmployeeRegisterState.CheckedIn)
                        {
                            record.State = EEmployeeRegisterState.CheckedOut;
                            var id = await _mockService.AddAsync(record);
                            if (id > 0)
                            {
                                result.SetSuccess(record.State);
                            }
                            else
                            {
                                result.SetFailure();
                            }
                        }
                        else if (lastRecord.State == EEmployeeRegisterState.CheckedOut)
                        {
                            record.State = EEmployeeRegisterState.CheckedIn;
                            var id = await _mockService.AddAsync(record);
                            if (id > 0)
                            {
                                result.SetSuccess(record.State);
                            }
                            else
                            {
                                result.SetFailure();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.SetError($"{nameof(InsertRecord)}: exception", "Error from LogService InsertRecord", ex);
                }
            }
            else
            {
                result.SetFailure();
            }

            return result;
        }
    }
}

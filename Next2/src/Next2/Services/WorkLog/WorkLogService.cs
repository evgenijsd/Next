using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Authentication;
using Next2.Services.Mock;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.WorkLog
{
    public class WorkLogService : IWorkLogService
    {
        private readonly IMockService _mockService;
        private readonly IAuthenticationService _authenticationService;

        public WorkLogService(
            IMockService mockService,
            IAuthenticationService authenticationService)
        {
            _mockService = mockService;
            _authenticationService = authenticationService;
        }

        #region -- IWorkLogService implementation  --

        public async Task<AOResult<EEmployeeRegisterState>> LogWorkTimeAsync(WorkLogRecordModel record)
        {
            var result = new AOResult<EEmployeeRegisterState>();

            try
            {
                var employeeId = record.EmployeeId.ToString();

                var resultOfGettingUser = await _authenticationService.GetUserById(employeeId);

                if (resultOfGettingUser.IsSuccess)
                {
                    var resultOfGettingLastRecord = await GetLastRecordAsync(record.EmployeeId);

                    if (resultOfGettingLastRecord.IsSuccess)
                    {
                        var lastRecord = resultOfGettingLastRecord.Result;

                        if (lastRecord.State == EEmployeeRegisterState.CheckedIn)
                        {
                            record.State = EEmployeeRegisterState.CheckedOut;
                        }
                        else
                        {
                            record.State = EEmployeeRegisterState.CheckedIn;
                        }
                    }
                    else
                    {
                        record.State = EEmployeeRegisterState.CheckedIn;
                    }

                    var resultOfInsertingRecord = await InsertRecordAsync(record);

                    if (resultOfInsertingRecord.IsSuccess)
                    {
                        result.SetSuccess(record.State);
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(LogWorkTimeAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion

        #region -- Private helpers --

        private async Task<AOResult<int>> InsertRecordAsync(WorkLogRecordModel record)
        {
            var result = new AOResult<int>();

            try
            {
                var id = await _mockService.AddAsync(record);

                if (id > 0)
                {
                    result.SetSuccess(id);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(InsertRecordAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        private async Task<AOResult<WorkLogRecordModel>> GetLastRecordAsync(int employeeId)
        {
            var result = new AOResult<WorkLogRecordModel>();

            try
            {
                var records = await _mockService.GetAllAsync<WorkLogRecordModel>();

                if (records is not null && records.Count() > 0)
                {
                    var lastRecord = records
                        .Where(record => record.EmployeeId == employeeId)
                        .OrderBy(record => record.Id)
                        .Last();

                    if (lastRecord is not null)
                    {
                        result.SetSuccess(lastRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetLastRecordAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}

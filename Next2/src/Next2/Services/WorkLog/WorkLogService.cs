using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Resources.Strings;
using Next2.Services.Authentication;
using Next2.Services.Mock;
using Next2.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Next2.Services.WorkLog
{
    public class WorkLogService : IWorkLogService
    {
        private readonly IRestService _restService;

        public WorkLogService(IRestService restService)
        {
            _restService = restService;
        }

        #region -- IWorkLogService implementation  --

        public async Task<AOResult<TimeTrackResult>> LogWorkTimeAsync(string employeeId)
        {
            var result = new AOResult<TimeTrackResult>();

            try
            {
                var timeTrackCommand = new TimeTrackCommand()
                {
                    EmployeeId = employeeId,
                };

                var query = $"{Constants.API.HOST_URL}/api/time-tracks";

                var resultOfGettingLastRecord = await _restService.RequestAsync<GenericExecutionResult<TimeTrackResult>>(HttpMethod.Post, query, timeTrackCommand);

                if (resultOfGettingLastRecord.Success && resultOfGettingLastRecord.Value is not null)
                {
                    result.SetSuccess(resultOfGettingLastRecord.Value);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(LogWorkTimeAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}
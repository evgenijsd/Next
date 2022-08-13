using Next2.Helpers.ProcessHelpers;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Resources.Strings;
using Next2.Services.Rest;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Next2.Services.Employees
{
    public class EmployeesService : IEmployeesService
    {
        private readonly IRestService _restService;

        public EmployeesService(IRestService restService)
        {
            _restService = restService;
        }

        #region -- IEmployeesService implementation --

        public async Task<AOResult<IEnumerable<EmployeeModelDTO>>> GetEmployeesAsync()
        {
            var result = new AOResult<IEnumerable<EmployeeModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/employees";

                var response = await _restService.RequestAsync<GenericExecutionResult<GetEmployeesListQueryResult>>(HttpMethod.Get, query);

                if (response.Success)
                {
                    if (response.Value?.Employees is not null)
                    {
                        result.SetSuccess(response.Value.Employees);
                    }
                    else if (response.Success)
                    {
                        result.SetSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetEmployeesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}

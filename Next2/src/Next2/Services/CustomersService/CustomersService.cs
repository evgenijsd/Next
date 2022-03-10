using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.CustomersService
{
    public class CustomersService : ICustomersService
    {
        private readonly IMockService _mockService;

        public CustomersService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- ICustomersSerice implementation --

        public async Task<AOResult<int>> AddNewCustomer(CustomerModel customer)
        {
            var result = new AOResult<int>();
            try
            {
                var response = await _mockService.AddAsync(customer);

                if (response != -1)
                {
                    result.SetSuccess(response);
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAllCustomersAsync)}: exception", "Some issues", ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<CustomerModel>>> GetAllCustomersAsync(Func<CustomerModel, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<CustomerModel>>();

            try
            {
                var customers = await _mockService.GetAllAsync<CustomerModel>();

                if (customers != null)
                {
                    result.SetSuccess(condition == null
                        ? customers
                        : customers.Where(condition));
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAllCustomersAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}

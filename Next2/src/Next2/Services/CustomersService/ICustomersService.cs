using InterTwitter.Helpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.CustomersService
{
    public interface ICustomersService
    {
        Task<AOResult<IEnumerable<CustomerModel>>> GetAllCustomersAsync(Func<CustomerModel, bool>? condition = null);
    }
}

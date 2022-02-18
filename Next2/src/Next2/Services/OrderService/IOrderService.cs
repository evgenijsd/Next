using Next2.Helpers;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Next2.Services.OrderService
{
    public interface IOrderService
    {
        Task<AOResult<IEnumerable<OrderModel>>> GetOrdersAsync();

        string ValidatorNumber(string text);

        string ValidatorName(string text);
    }
}

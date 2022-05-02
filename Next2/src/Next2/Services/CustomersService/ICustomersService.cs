using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.CustomersService
{
    public interface ICustomersService
    {
        Task<AOResult<IEnumerable<CustomerModel>>> GetAllCustomersAsync(Func<CustomerModel, bool>? condition = null);

        Task<AOResult<int>> AddNewCustomerAsync(CustomerModel customer);

        Task<AOResult<GiftCardModel>> IsGiftCardExistsAsync(int giftCardNumber);

        Task<AOResult> ActivateGiftCardAsync(GiftCardModel giftCard);

        Task<AOResult> AddGiftCardToCustomerAsync(CustomerModel customer, GiftCardModel giftCard);
    }
}

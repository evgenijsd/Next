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

        Task<AOResult<GiftCardModel>> IsGiftCardExists(int giftCardNumber);

        Task<AOResult> RemoveGiftCardFromUnregisteredGiftCardsDateBase(GiftCardModel giftCard);

        Task<AOResult> AddGiftCardToCustomer(CustomerModel customer, GiftCardModel giftCard);

        Task<AOResult<CustomerModel>> GetSingleCustomer(CustomerModel customer);
    }
}

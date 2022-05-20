using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.CustomersService
{
    public interface ICustomersService
    {
        Task<AOResult<IEnumerable<CustomerBindableModel>>> GetAllCustomersAsync(Func<CustomerBindableModel, bool>? condition = null);

        Task<AOResult<CustomerBindableModel>> GetCustomerByIdAsync(Guid id);

        Task<AOResult<Guid>> AddNewCustomerAsync(CustomerBindableModel customer);

        Task<AOResult<GiftCardModel>> GetGiftCardByNumberAsync(int giftCardNumber);

        Task<AOResult> ActivateGiftCardAsync(GiftCardModel giftCard);

        Task<AOResult> AddGiftCardToCustomerAsync(CustomerBindableModel customer, GiftCardModel giftCard);

        Task<AOResult> UpdateGiftCardAsync(GiftCardModel giftCard);

        Task<AOResult> UpdateCustomerAsync(CustomerBindableModel customer);

        Task<AOResult<CustomerBindableModel>> GetInfoAboutGiftCards(CustomerBindableModel customer);
    }
}

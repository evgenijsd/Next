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
        Task<AOResult<IEnumerable<CustomerModelDTO>>> GetAllCustomersAsync(Func<CustomerModelDTO, bool>? condition = null);

        Task<AOResult<CustomerModelDTO>> GetCustomerByIdAsync(Guid id);

        Task<AOResult<Guid>> AddNewCustomerAsync(CustomerModelDTO customer);

        Task<AOResult<GiftCardModel>> GetGiftCardByNumberAsync(int giftCardNumber);

        Task<AOResult> ActivateGiftCardAsync(GiftCardModel giftCard);

        Task<AOResult> AddGiftCardToCustomerAsync(CustomerModelDTO customer, GiftCardModel giftCard);

        Task<AOResult> UpdateGiftCardAsync(GiftCardModel giftCard);

        Task<AOResult> UpdateCustomerAsync(CustomerModelDTO customer);

        Task<AOResult<CustomerBindableModel>> GetInfoAboutGiftCards(CustomerBindableModel customer);
    }
}

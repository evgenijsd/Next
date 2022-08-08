using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Customers
{
    public interface ICustomersService
    {
        Task<AOResult<IEnumerable<CustomerBindableModel>>> GetCustomersAsync(Func<CustomerBindableModel, bool>? condition = null);

        Task<AOResult<CustomerBindableModel>> GetCustomerByIdAsync(Guid id);

        Task<AOResult<Guid>> CreateCustomerAsync(CustomerModelDTO customer);

        Task<AOResult<GiftCardModelDTO>> GetGiftCardByNumberAsync(string giftCardNumber);

        Task<AOResult> AddGiftCardToCustomerAsync(CustomerModelDTO customer, GiftCardModelDTO giftCard);

        Task<AOResult> UpdateGiftCardAsync(GiftCardModelDTO giftCard);

        Task<AOResult> UpdateCustomerAsync(CustomerModelDTO customer);

        Task<AOResult<IEnumerable<GiftCardModelDTO>>> GetGiftCardsCustomerAsync(IEnumerable<Guid> guids);

        Task<AOResult<GiftCardModelDTO>> GetGiftCardByIdAsync(Guid id);

        List<GiftCardModelDTO> RecalculateCustomerGiftCardFounds(IEnumerable<GiftCardModelDTO> giftCards, decimal amountToBeWithdrawn);

        Task<AOResult> UpdateAllGiftCardsAsync(IEnumerable<GiftCardModelDTO> giftCards);

        List<GiftCardModelDTO> GetSelectedGiftCardsFromBackup(IEnumerable<GiftCardModelDTO> source, IEnumerable<GiftCardModelDTO> selectedBackUp);
    }
}

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

        Task<AOResult<Guid>> CreateCustomerAsync(CustomerBindableModel customer);

        Task<AOResult<GiftCardModelDTO>> GetGiftCardByNumberAsync(string giftCardNumber);

        Task<AOResult> AddGiftCardToCustomerAsync(UpdateCustomerCommand customer, GiftCardModelDTO giftCard);

        Task<AOResult> UpdateGiftCardAsync(GiftCardModelDTO giftCard);

        Task<AOResult> UpdateCustomerAsync(UpdateCustomerCommand customer);

        Task<AOResult<IEnumerable<GiftCardModelDTO>>> GetGiftCardsCustomerAsync(IEnumerable<Guid> guids);

        Task<AOResult<GiftCardModelDTO>> GetGiftCardByIdAsync(Guid id);

        List<Guid> RecalculateCustomerGiftCardFounds(List<GiftCardModelDTO> giftCards, decimal amountToBeWithdrawn);

        Task<AOResult> UpdateAllGiftCardsAsync(List<GiftCardModelDTO> giftCards, List<Guid> giftCardsIdToBeUpdated, List<GiftCardModelDTO> tempGiftCards);

        Task<AOResult> UpdateAllGiftCardsToPreviousStageAsync(List<GiftCardModelDTO> giftCards, List<Guid> giftCardsIdToBeUpdated);
    }
}

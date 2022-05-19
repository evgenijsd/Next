using Next2.Helpers.DTO.Customers;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API;
using Next2.Models.API.DTO;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using Next2.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Next2.Services.CustomersService
{
    public class CustomersService : ICustomersService
    {
        private readonly IMockService _mockService;
        private readonly IRestService _restService;

        public CustomersService(
            IMockService mockService,
            IRestService restService)
        {
            _mockService = mockService;
            _restService = restService;
        }

        #region -- ICustomersSerice implementation --

        public async Task<AOResult<Guid>> AddNewCustomerAsync(CustomerModelDTO customer)
        {
            var result = new AOResult<Guid>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<Guid>>(HttpMethod.Post, $"{Constants.API.HOST_URL}/api/customers", customer);

                if (response.Success)
                {
                    result.SetSuccess(response.Value);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddNewCustomerAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<CustomerModelDTO>> GetCustomerByIdAsync(Guid id)
        {
            var result = new AOResult<CustomerModelDTO>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<GetCustomerByIdQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/customers/{id}");

                if (response.Success)
                {
                    result.SetSuccess(response.Value.Customer);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddNewCustomerAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> UpdateCustomerAsync(CustomerModelDTO customer)
        {
            var result = new AOResult();

            try
            {
                if (customer is not null)
                {
                    customer.GiftCardsTotalFund = customer.GiftCards.Sum(row => row.GiftCardFunds);
                    customer.GiftCardsCount = customer.GiftCards.Count();

                    var response = await _restService.RequestAsync<ExecutionResult>(HttpMethod.Put, $"{Constants.API.HOST_URL}/api/customers", customer);

                    if (response.Success)
                    {
                        result.SetSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateCustomerAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<CustomerModelDTO>>> GetAllCustomersAsync(Func<CustomerModelDTO, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<CustomerModelDTO>>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<GetCustomersListQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/customers");
                var mockCustomers = CustomersMock.Create();
                var dtoCustomers = response?.Value?.Customers;

                var customers = MergeDTOModelsWithMocksModels(dtoCustomers, mockCustomers);

                if (customers is not null)
                {
                    result.SetSuccess(condition == null
                        ? customers
                        : customers.Where(condition));
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAllCustomersAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> AddGiftCardToCustomerAsync(CustomerModelDTO customer, GiftCardModel giftCard)
        {
            var result = new AOResult();

            try
            {
                if (!customer.GiftCards.Contains(giftCard))
                {
                    giftCard.IsRegistered = true;
                    customer.GiftCards.Add(giftCard);
                    customer.GiftCardsTotalFund = customer.GiftCards.Sum(row => row.GiftCardFunds);
                    customer.GiftCardsCount = customer.GiftCards.Count();
                    customer.IsUpdatedCustomer = true;

                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddGiftCardToCustomerAsync)}: exception", "The giftcard in-use", ex);
            }

            return result;
        }

        public async Task<AOResult> ActivateGiftCardAsync(GiftCardModel giftCard)
        {
            var result = new AOResult();

            try
            {
                var isGiftCardRemoved = await _mockService.RemoveAsync(giftCard);

                if (isGiftCardRemoved)
                {
                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(ActivateGiftCardAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<GiftCardModel>> GetGiftCardByNumberAsync(int giftCardNumber)
        {
            var result = new AOResult<GiftCardModel>();

            try
            {
                var giftCard = await _mockService.FindAsync<GiftCardModel>(x => x.GiftCardNumber == giftCardNumber);

                if (giftCard is not null)
                {
                    result.SetSuccess(giftCard);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetGiftCardByNumberAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> UpdateGiftCardAsync(GiftCardModel giftCard)
        {
            var result = new AOResult();

            try
            {
                var giftCardId = await _mockService.UpdateAsync(giftCard);

                if (giftCardId is not null)
                {
                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateGiftCardAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion

        #region -- Private Helpers --

        private IEnumerable<CustomerModelDTO>? MergeDTOModelsWithMocksModels(IEnumerable<CustomerModelDTO> modelsDTO, IEnumerable<CustomerModelDTO> mockModels)
        {
            IEnumerable<CustomerModelDTO>? result = null;

            if (modelsDTO is not null || mockModels is not null)
            {
                var dtoModelsArray = modelsDTO.ToArray();
                var mockModelsArray = mockModels.ToArray();

                for (int i = 0; i < dtoModelsArray.Length && i < mockModelsArray.Length; i++)
                {
                    dtoModelsArray[i].Rewards = mockModelsArray[i].Rewards;
                    dtoModelsArray[i].Points = mockModelsArray[i].Points;
                    dtoModelsArray[i].IsUpdatedCustomer = mockModelsArray[i].IsUpdatedCustomer;
                    dtoModelsArray[i].GiftCardsCount = mockModelsArray[i].GiftCardsCount;
                    dtoModelsArray[i].GiftCardsTotalFund = mockModelsArray[i].GiftCardsTotalFund;
                    dtoModelsArray[i].GiftCardsTotalFund = mockModelsArray[i].GiftCardsTotalFund;
                    dtoModelsArray[i].GiftCards = mockModelsArray[i].GiftCards;
                    dtoModelsArray[i].IsNotRegistratedCustomer = mockModelsArray[i].IsNotRegistratedCustomer;
                    dtoModelsArray[i].IsUpdatedCustomer = mockModelsArray[i].IsUpdatedCustomer;
                }

                result = dtoModelsArray;
            }

            return result;
        }

        #endregion

    }
}

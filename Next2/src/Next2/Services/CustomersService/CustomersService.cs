using Newtonsoft.Json.Linq;
using Next2.Helpers.DTO;
using Next2.Helpers.DTO.Customers;
using Next2.Helpers.Extensions;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
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

        public async Task<AOResult<int>> AddNewCustomerAsync(CustomerModel customer)
        {
            var result = new AOResult<int>();

            try
            {
                var response = await _mockService.AddAsync(customer);

                if (response != -1)
                {
                    result.SetSuccess(response);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddNewCustomerAsync)}: exception", "Some issues", ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<CustomerModel>>> GetAllCustomersAsync(Func<CustomerModel, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<CustomerModel>>();

            try
            {
                var header = _restService.GenerateAuthorizationHeader(null);
                var response = await _restService.RequestAsync<GenericExecutionResult<GetCustomersListQuery>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/customers", header);
                var mockCustomers = await _mockService.GetAllAsync<CustomerModel>();
                var dtoCustomers = response?.Value?.Customers;

                var customers = MergeDTOModelsWithMocksModels(dtoCustomers, mockCustomers);

                if (customers is not null)
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

        public async Task<AOResult> AddGiftCardToCustomerAsync(CustomerModel customer, GiftCardModel giftCard)
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
                else
                {
                    result.SetFailure();
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
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(ActivateGiftCardAsync)}: exception", "Some issues", ex);
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
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetGiftCardByNumberAsync)}: exception", "Some issues", ex);
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
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateGiftCardAsync)}: exception", "Some issues", ex);
            }

            return result;
        }

        public async Task<AOResult> UpdateCustomerAsync(CustomerModel customer)
        {
            var result = new AOResult();

            try
            {
                if (customer is not null)
                {
                    customer.GiftCardsTotalFund = customer.GiftCards.Sum(row => row.GiftCardFunds);
                    customer.GiftCardsCount = customer.GiftCards.Count();

                    var customerModel = await _mockService.UpdateAsync(customer);

                    if (customerModel is not null)
                    {
                        result.SetSuccess();
                    }
                    else
                    {
                        result.SetFailure();
                    }
                }
                else
                {
                    result.SetFailure();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateCustomerAsync)}: exception", "Some issues", ex);
            }

            return result;
        }

        #endregion

        #region -- Private Helpers --

        private IEnumerable<CustomerModel>? MergeDTOModelsWithMocksModels(IEnumerable<CustomerModelDTO> modelDTOs, IEnumerable<CustomerModel> mockModels)
        {
            IEnumerable<CustomerModel>? result = null;

            if (modelDTOs is not null || mockModels is not null)
            {
                var dtoModelsArray = modelDTOs.ToArray();
                var mockModelsArray = mockModels.ToArray();

                for (int i = 0; i < dtoModelsArray.Length && i < mockModelsArray.Length; i++)
                {
                    mockModelsArray[i] = dtoModelsArray[i].MergeWithUIModel(mockModelsArray[i]);
                }

                result = mockModelsArray;
            }

            return result;
        }

        #endregion

    }
}

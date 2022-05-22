using AutoMapper;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.Api.Commands;
using Next2.Models.Api.DTO;
using Next2.Models.Api.Results;
using Next2.Models.API;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
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
        private readonly IRestService _restService;
        private readonly IMapper _mapper;

        public CustomersService(
            IMapper mapper,
            IRestService restService)
        {
            _restService = restService;
            _mapper = mapper;
        }

        #region -- ICustomersSerice implementation --

        #region -- Customers Region --

        public async Task<AOResult<Guid>> AddNewCustomerAsync(CustomerBindableModel customer)
        {
            var result = new AOResult<Guid>();
            var customerModelDTO = _mapper.Map<CustomerModelDTO>(customer);

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<Guid>>(HttpMethod.Post, $"{Constants.API.HOST_URL}/api/customers", customerModelDTO);

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

        public async Task<AOResult<CustomerBindableModel>> GetCustomerByIdAsync(Guid id)
        {
            var result = new AOResult<CustomerBindableModel>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<GetCustomerByIdQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/customers/{id}");
                var customer = _mapper.Map<CustomerBindableModel>(response?.Value?.Customer);

                if (response.Success)
                {
                    result.SetSuccess(customer);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddNewCustomerAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> UpdateCustomerAsync(CustomerBindableModel customer)
        {
            var result = new AOResult();
            var customerModelDTO = _mapper.Map<CustomerModelDTO>(customer);

            try
            {
                if (customer is not null)
                {
                    var response = await _restService.RequestAsync<ExecutionResult>(HttpMethod.Put, $"{Constants.API.HOST_URL}/api/customers", customerModelDTO);

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

        public async Task<AOResult<IEnumerable<CustomerBindableModel>>> GetAllCustomersAsync(Func<CustomerBindableModel, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<CustomerBindableModel>>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<GetCustomersListQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/customers");
                var mockCustomers = CustomersMock.Create();
                var dtoCustomers = response?.Value?.Customers;
                var dtoCustomersBM = _mapper.Map<IEnumerable<CustomerBindableModel>>(dtoCustomers);

                var customers = MergeDTOModelsWithMocksModels(dtoCustomersBM, mockCustomers);

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

        public async Task<AOResult<CustomerBindableModel>> GetFullGiftCardsDataAsync(CustomerBindableModel customer)
        {
            var result = new AOResult<CustomerBindableModel>();

            try
            {
                if (customer.GiftCardsId?.Count() > 0 && customer.GiftCards.Count == 0)
                {
                    var giftCards = new List<GiftCardModelDTO>();

                    foreach (Guid giftCardId in customer.GiftCardsId)
                    {
                        var res = await GetGiftCardByIdAsync(giftCardId);
                        if (res.IsSuccess)
                        {
                            giftCards.Add(res.Result);
                        }
                    }

                    customer.GiftCards = giftCards;
                    result.SetSuccess(customer);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(ActivateGiftCardAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion

        #region -- Gift Card Region --

        public async Task<AOResult> AddGiftCardToCustomerAsync(CustomerBindableModel customer, GiftCardModelDTO giftCard)
        {
            var result = new AOResult();

            try
            {
                if (!customer.GiftCardsId.Contains(giftCard.Id))
                {
                    customer.GiftCardsId = customer.GiftCardsId.Append(giftCard.Id);
                    var updateResult = await UpdateCustomerAsync(customer);

                    if (updateResult.IsSuccess)
                    {
                        result.SetSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddGiftCardToCustomerAsync)}: exception", "The giftcard in-use", ex);
            }

            return result;
        }

        public async Task<AOResult> ActivateGiftCardAsync(GiftCardModelDTO giftCard)
        {
            var result = new AOResult();

            try
            {
                if (true)
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

        public async Task<AOResult<GiftCardModelDTO>> GetGiftCardByNumberAsync(string giftCardNumber)
        {
            var result = new AOResult<GiftCardModelDTO>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<GetGiftCardQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/gift-cards/{giftCardNumber}");
                if (response.Success)
                {
                    var giftcard = response.Value.GiftCard;
                    result.SetSuccess(giftcard);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetGiftCardByNumberAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<GiftCardModelDTO>> GetGiftCardByIdAsync(Guid id)
        {
            var result = new AOResult<GiftCardModelDTO>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<GetGiftCardQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/gift-cards/{id}");
                if (response.Success)
                {
                    var giftcard = response.Value.GiftCard;

                    result.SetSuccess(giftcard);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetGiftCardByNumberAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> UpdateGiftCardAsync(GiftCardModelDTO giftCard)
        {
            var result = new AOResult();
            UpdateGiftCardCommand updateGiftCardCommand = new();

            if (giftCard is not null)
            {
                updateGiftCardCommand = _mapper.Map<UpdateGiftCardCommand>(giftCard);
                updateGiftCardCommand.GiftCardNumber = giftCard.GiftCardNumber.ToString();
            }

            try
            {
                var response = await _restService.RequestAsync<ExecutionResult>(HttpMethod.Put, $"{Constants.API.HOST_URL}/api/gift-cards");

                if (response.Success)
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

        #endregion

        #region -- Private Helpers --

        private IEnumerable<CustomerBindableModel>? MergeDTOModelsWithMocksModels(IEnumerable<CustomerBindableModel> modelsDTO, IEnumerable<CustomerBindableModel> mockModels)
        {
            IEnumerable<CustomerBindableModel>? result = null;

            if (modelsDTO is not null || mockModels is not null)
            {
                var dtoModelsArray = modelsDTO.ToArray();
                var mockModelsArray = mockModels.ToArray();

                for (int i = 0; i < dtoModelsArray.Length && i < mockModelsArray.Length; i++)
                {
                    dtoModelsArray[i].Rewards = mockModelsArray[i].Rewards;
                    dtoModelsArray[i].Points = mockModelsArray[i].Points;
                }

                result = dtoModelsArray;
            }

            return result;
        }

        #endregion

    }
}

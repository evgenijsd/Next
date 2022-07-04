using AutoMapper;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.Commands;
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

namespace Next2.Services.Customers
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

        #region -- ICustomersService implementation --

        public async Task<AOResult<Guid>> CreateCustomerAsync(CustomerBindableModel bindableCustomer)
        {
            var result = new AOResult<Guid>();

            var customer = _mapper.Map<CustomerModelDTO>(bindableCustomer);

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/customers";

                var response = await _restService.RequestAsync<GenericExecutionResult<Guid>>(HttpMethod.Post, query, customer);

                if (response.Success)
                {
                    result.SetSuccess(response.Value);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(CreateCustomerAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<CustomerBindableModel>> GetCustomerByIdAsync(Guid id)
        {
            var result = new AOResult<CustomerBindableModel>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/customers/{id}";

                var response = await _restService.RequestAsync<GenericExecutionResult<GetCustomerByIdQueryResult>>(HttpMethod.Get, query);

                if (response.Success && response.Value is not null)
                {
                    var customer = _mapper.Map<CustomerBindableModel>(response.Value.Customer);

                    result.SetSuccess(customer);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetCustomerByIdAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> UpdateCustomerAsync(CustomerBindableModel customer)
        {
            var result = new AOResult();

            try
            {
                if (customer is not null)
                {
                    var customerForUpdate = _mapper.Map<UpdateCustomerCommand>(customer);

                    var query = $"{Constants.API.HOST_URL}/api/customers";

                    var response = await _restService.RequestAsync<ExecutionResult>(HttpMethod.Put, query, customerForUpdate);

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

        public async Task<AOResult<IEnumerable<CustomerBindableModel>>> GetCustomersAsync(Func<CustomerBindableModel, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<CustomerBindableModel>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/customers";

                var response = await _restService.RequestAsync<GenericExecutionResult<GetCustomersListQueryResult>>(HttpMethod.Get, query);

                if (response.Success && response.Value is not null)
                {
                    var mockCustomers = CustomersMock.Create();
                    var gettingCustomers = response.Value.Customers;

                    var bindableCustomers = _mapper.Map<IEnumerable<CustomerBindableModel>>(gettingCustomers);

                    var customers = MergeDTOModelsWithMocksModels(bindableCustomers, mockCustomers);

                    if (customers is not null)
                    {
                        result.SetSuccess(condition == null
                            ? customers
                            : customers.Where(condition));
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetCustomersAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<IEnumerable<GiftCardModelDTO>>> GetGiftCardsCustomerAsync(IEnumerable<Guid>? guids)
        {
            var result = new AOResult<IEnumerable<GiftCardModelDTO>>();

            try
            {
                if (guids?.Count() > 0)
                {
                    var giftCards = new List<GiftCardModelDTO>();

                    foreach (Guid giftCardId in guids)
                    {
                        var resultOfGettingGiftCard = await GetGiftCardByIdAsync(giftCardId);

                        if (resultOfGettingGiftCard.IsSuccess)
                        {
                            giftCards.Add(resultOfGettingGiftCard.Result);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (giftCards.Count == guids.Count())
                    {
                        result.SetSuccess(giftCards);
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetGiftCardsCustomerAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

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
                result.SetError($"{nameof(AddGiftCardToCustomerAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<GiftCardModelDTO>> GetGiftCardByNumberAsync(string giftCardNumber)
        {
            var result = new AOResult<GiftCardModelDTO>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/gift-cards/{giftCardNumber}";

                var response = await _restService.RequestAsync<GenericExecutionResult<GetGiftCardQueryResult>>(HttpMethod.Get, query);

                if (response.Success && response.Value is not null)
                {
                    result.SetSuccess(response.Value.GiftCard);
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
                var query = $"{Constants.API.HOST_URL}/api/gift-cards/{id}";

                var response = await _restService.RequestAsync<GenericExecutionResult<GetGiftCardQueryResult>>(HttpMethod.Get, query);

                if (response.Success && response.Value is not null)
                {
                    result.SetSuccess(response.Value.GiftCard);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetGiftCardByIdAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> UpdateGiftCardAsync(GiftCardModelDTO? giftCard)
        {
            var result = new AOResult();

            if (giftCard is not null)
            {
                var updateGiftCardCommand = _mapper.Map<UpdateGiftCardCommand>(giftCard);

                try
                {
                    var query = $"{Constants.API.HOST_URL}/api/gift-cards";

                    var response = await _restService.RequestAsync<ExecutionResult>(HttpMethod.Put, query, updateGiftCardCommand);

                    if (response.Success)
                    {
                        result.SetSuccess();
                    }
                }
                catch (Exception ex)
                {
                    result.SetError($"{nameof(UpdateGiftCardAsync)}: exception", Strings.SomeIssues, ex);
                }
            }

            return result;
        }

        #endregion

        #region -- Private helpers --

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

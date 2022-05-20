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
                    customer.GiftCardsTotalFund = customer.GiftCards.Sum(row => row.TotalBalance);
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

        public async Task<AOResult<CustomerModelDTO>> GetInfoAboutGiftCards(CustomerModelDTO customer)
        {
            var result = new AOResult<CustomerModelDTO>();

            try
            {
                if (customer.GiftCardsId is not null)
                {
                    var giftCards = new List<GiftCardModel>();

                    foreach (Guid giftCardId in customer.GiftCardsId)
                    {
                        var res = await GetGiftCardByIdAsync(giftCardId);
                        if (res.IsSuccess)
                        {
                            giftCards.Add(res.Result);
                        }
                    }

                    if (giftCards.Count > 0)
                    {
                        customer.GiftCards = giftCards;
                        customer.GiftCardsCount = giftCards.Count;
                        customer.GiftCardsTotalFund = giftCards.Sum(x => x.TotalBalance);
                        result.SetSuccess(customer);
                    }
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

        public async Task<AOResult> AddGiftCardToCustomerAsync(CustomerModelDTO customer, GiftCardModel giftCard)
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

        public async Task<AOResult> ActivateGiftCardAsync(GiftCardModel giftCard)
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

        public async Task<AOResult<GiftCardModel>> GetGiftCardByNumberAsync(int giftCardNumber)
        {
            var result = new AOResult<GiftCardModel>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<GetGiftCardQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/gift-cards/{giftCardNumber}");
                if (response.Success)
                {
                    var giftcardDTO = response.Value.GiftCard;
                    var giftcard = _mapper.Map<GiftCardModel>(giftcardDTO);
                    giftcard.GiftCardNumber = int.Parse(giftcardDTO.GiftCardNumber);

                    result.SetSuccess(giftcard);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetGiftCardByNumberAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult<GiftCardModel>> GetGiftCardByIdAsync(Guid id)
        {
            var result = new AOResult<GiftCardModel>();

            try
            {
                var response = await _restService.RequestAsync<GenericExecutionResult<GetGiftCardQueryResult>>(HttpMethod.Get, $"{Constants.API.HOST_URL}/api/gift-cards/{id}");
                if (response.Success)
                {
                    var giftcardDTO = response.Value.GiftCard;
                    var giftcard = _mapper.Map<GiftCardModel>(giftcardDTO);
                    giftcard.GiftCardNumber = int.Parse(giftcardDTO.GiftCardNumber);

                    result.SetSuccess(giftcard);
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

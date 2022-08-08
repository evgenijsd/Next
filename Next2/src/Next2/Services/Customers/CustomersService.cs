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

        public async Task<AOResult<Guid>> CreateCustomerAsync(CustomerModelDTO customer)
        {
            var result = new AOResult<Guid>();

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

        public async Task<AOResult> UpdateCustomerAsync(CustomerModelDTO customer)
        {
            var result = new AOResult();

            try
            {
                if (customer is not null)
                {
                    var query = $"{Constants.API.HOST_URL}/api/customers";

                    var customerCommand = _mapper.Map<UpdateCustomerCommand>(customer);
                    customerCommand.GiftCardsId = customer.GiftCards.Select(row => row.Id);

                    var response = await _restService.RequestAsync<ExecutionResult>(HttpMethod.Put, query, customerCommand);

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

        public async Task<AOResult> AddGiftCardToCustomerAsync(CustomerModelDTO customer, GiftCardModelDTO giftCard)
        {
            var result = new AOResult();

            try
            {
                if (!customer.GiftCards.Any(row => row.Id == giftCard.Id))
                {
                    customer.GiftCards = customer.GiftCards.Concat(new[] { giftCard });

                    result = await UpdateCustomerAsync(customer);
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

        public async Task<AOResult> UpdateAllGiftCardsAsync(IEnumerable<GiftCardModelDTO> giftCardsTobeUpdate)
        {
            var result = new AOResult();

            try
            {
                if (giftCardsTobeUpdate is not null)
                {
                    foreach (var giftCard in giftCardsTobeUpdate)
                    {
                        result = await UpdateGiftCardAsync(giftCard);

                        if (!result.IsSuccess)
                        {
                            throw result.Exception;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateAllGiftCardsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public List<GiftCardModelDTO> RecalculateCustomerGiftCardFounds(IEnumerable<GiftCardModelDTO> giftCards, decimal amountToBeWithdrawn)
        {
            List<GiftCardModelDTO> listOfUpdatedGiftCards = new();

            foreach (var giftCard in giftCards)
            {
                if (giftCard.TotalBalance != 0)
                {
                    if (amountToBeWithdrawn != 0)
                    {
                        if (giftCard.TotalBalance > amountToBeWithdrawn)
                        {
                            giftCard.TotalBalance -= amountToBeWithdrawn;
                            amountToBeWithdrawn = 0;
                        }
                        else if (giftCard.TotalBalance < amountToBeWithdrawn)
                        {
                            amountToBeWithdrawn -= giftCard.TotalBalance;
                            giftCard.TotalBalance = 0;
                        }
                        else if (giftCard.TotalBalance == amountToBeWithdrawn)
                        {
                            giftCard.TotalBalance = 0;
                            amountToBeWithdrawn = 0;
                        }

                        listOfUpdatedGiftCards.Add(giftCard);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return listOfUpdatedGiftCards;
        }

        public List<GiftCardModelDTO> GetSelectedGiftCardsFromBackup(IEnumerable<GiftCardModelDTO>? source, IEnumerable<GiftCardModelDTO> selectedGiftCards)
        {
            List<GiftCardModelDTO> listGiftCardsToBeUpdateToPreviousStage = new();

            if (source is not null)
            {
                foreach (var backupGiftCard in source)
                {
                    if (listGiftCardsToBeUpdateToPreviousStage.Count() != selectedGiftCards.Count())
                    {
                        if (selectedGiftCards.Any(row => row.Id == backupGiftCard.Id))
                        {
                            listGiftCardsToBeUpdateToPreviousStage.Add(backupGiftCard);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return listGiftCardsToBeUpdateToPreviousStage;
        }

        #endregion

        #region -- Private helpers --

        private IEnumerable<CustomerBindableModel>? MergeDTOModelsWithMocksModels(IEnumerable<CustomerBindableModel>? modelsDTO, IEnumerable<CustomerBindableModel>? mockModels)
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

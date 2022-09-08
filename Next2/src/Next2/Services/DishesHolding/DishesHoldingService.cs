using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.API.Results;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using Next2.Services.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Next2.Services.DishesHolding
{
    public class DishesHoldingService : IDishesHoldingService
    {
        private readonly IRestService _restService;

        private IEnumerable<HoldItemModelDTO> _allHoldDishes = Enumerable.Empty<HoldItemModelDTO>();

        public DishesHoldingService(IRestService restService)
        {
            _restService = restService;
        }

        #region -- IDishesHoldingService implementation --

        public IEnumerable<HoldItemModelDTO> GetHoldDishesByTableNumber(int tableNumber)
        {
            var result = _allHoldDishes;

            if (tableNumber > 0)
            {
                result = _allHoldDishes.Where(x => x.TableNumber == tableNumber);
            }

            return result;
        }

        public IEnumerable<HoldDishBindableModel> GetSortedHoldDishes(EHoldDishesSortingType typeSort, IEnumerable<HoldDishBindableModel> holdDishes)
        {
            Func<HoldDishBindableModel, object> sortingSelector = typeSort switch
            {
                EHoldDishesSortingType.ByTableNumber => x => x.TableNumber,
                EHoldDishesSortingType.ByDishName => x => x.Name,
                _ => throw new NotImplementedException(),
            };

            return holdDishes.OrderBy(sortingSelector);
        }

        public async Task<AOResult<IEnumerable<HoldItemModelDTO>>> GetHoldDishesAsync(Func<HoldItemModelDTO, bool>? condition = null)
        {
            var result = new AOResult<IEnumerable<HoldItemModelDTO>>();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/hold-items";

                var response = await _restService.RequestAsync<GenericExecutionResult<GetHoldItemsListQueryResult>>(HttpMethod.Get, query);

                if (response.Success && response.Value is not null)
                {
                    var holdDishes = response.Value.HoldItems;

                    if (holdDishes is not null)
                    {
                        _allHoldDishes = condition == null
                            ? holdDishes
                            : holdDishes.Where(condition);

                        result.SetSuccess(_allHoldDishes.OrderBy(x => x.TableNumber));
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetHoldDishesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> UpdateHoldItemsAsync(UpdateHoldItemsCommand holdItems)
        {
            var result = new AOResult();

            try
            {
                var query = $"{Constants.API.HOST_URL}/api/hold-items";

                var updateResult = await _restService.RequestAsync<ExecutionResult>(HttpMethod.Put, query, holdItems);

                if (updateResult.Success)
                {
                    result.SetSuccess();
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateHoldItemsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}

using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.DishesHolding
{
    public class DishesHoldingService : IDishesHoldingService
    {
        private readonly IMockService _mockService;

        private IEnumerable<HoldDishModel> _allHoldDishes = Enumerable.Empty<HoldDishModel>();

        public DishesHoldingService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- IDishesHoldingService implementation --

        public IEnumerable<HoldDishModel> GetHoldDishesByTableNumber(int tableNumber)
        {
            var result = _allHoldDishes;

            if (tableNumber != 0)
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
                EHoldDishesSortingType.ByDishName => x => x.DishName,
                _ => throw new NotImplementedException(),
            };

            return holdDishes.OrderBy(sortingSelector);
        }

        public async Task<AOResult<IEnumerable<HoldDishModel>>> GetAllHoldDishesAsync()
        {
            var result = new AOResult<IEnumerable<HoldDishModel>>();

            try
            {
                _allHoldDishes = await _mockService.GetAllAsync<HoldDishModel>();

                if (_allHoldDishes is not null)
                {
                    _allHoldDishes = _allHoldDishes.OrderBy(x => x.TableNumber);

                    result.SetSuccess(_allHoldDishes);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAllHoldDishesAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}

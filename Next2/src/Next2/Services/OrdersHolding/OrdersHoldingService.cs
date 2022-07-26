using AutoMapper;
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

namespace Next2.Services.OrdersHolding
{
    public class OrdersHoldingService : IOrdersHoldingService
    {
        private readonly IMockService _mockService;

        private IEnumerable<HoldDishModel> _allHoldItems = Enumerable.Empty<HoldDishModel>();

        public OrdersHoldingService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- IHoldItemService implementation --

        public IEnumerable<HoldDishModel> GetHoldDishesByTableNumber(int tableNumber)
        {
            var result = _allHoldItems;

            if (tableNumber != 0)
            {
                result = _allHoldItems.Where(x => x.TableNumber == tableNumber);
            }

            return result;
        }

        public IEnumerable<HoldDishBindableModel> GetSortedHoldDishes(EHoldDishesSortingType typeSort, IEnumerable<HoldDishBindableModel> holdItems)
        {
            Func<HoldDishBindableModel, object> sortingSelector = typeSort switch
            {
                EHoldDishesSortingType.ByTableNumber => x => x.TableNumber,
                EHoldDishesSortingType.ByDishName => x => x.DishName,
                _ => throw new NotImplementedException(),
            };

            return holdItems.OrderBy(sortingSelector);
        }

        public async Task<AOResult<IEnumerable<HoldDishModel>>> GetAllHoldDishesAsync()
        {
            var result = new AOResult<IEnumerable<HoldDishModel>>();

            try
            {
                _allHoldItems = await _mockService.GetAllAsync<HoldDishModel>();

                if (_allHoldItems is not null)
                {
                    _allHoldItems = _allHoldItems.OrderBy(x => x.TableNumber);

                    result.SetSuccess(_allHoldItems);
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

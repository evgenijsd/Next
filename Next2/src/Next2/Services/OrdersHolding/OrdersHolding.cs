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
    public class OrdersHolding : IOrdersHolding
    {
        private readonly IMockService _mockService;

        private IEnumerable<HoldDishModel> _allHoldItems = Enumerable.Empty<HoldDishModel>();

        public OrdersHolding(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- IHoldItemService implementation --

        public IEnumerable<HoldDishModel> GetHoldItemsByTableNumber(int tableNumber)
        {
            var result = _allHoldItems;

            if (tableNumber != 0)
            {
                result = _allHoldItems.Where(x => x.TableNumber == tableNumber);
            }

            return result;
        }

        public IEnumerable<HoldDishBindableModel> GetSortedHoldItems(EHoldItemsSortingType typeSort, IEnumerable<HoldDishBindableModel> holdItems)
        {
            Func<HoldDishBindableModel, object> sortingSelector = typeSort switch
            {
                EHoldItemsSortingType.ByTableNumber => x => x.TableNumber,
                EHoldItemsSortingType.ByItem => x => x.DishName,
                _ => throw new NotImplementedException(),
            };

            return holdItems.OrderBy(sortingSelector);
        }

        public async Task<AOResult<IEnumerable<HoldDishModel>>> GetAllHoldItemsAsync()
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
                result.SetError($"{nameof(GetAllHoldItemsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}

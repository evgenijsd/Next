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

namespace Next2.Services.HoldItem
{
    public class HoldItemService : IHoldItemService
    {
        private readonly IMockService _mockService;

        private IEnumerable<HoldItemModel> _allHoldItems = Enumerable.Empty<HoldItemModel>();

        public HoldItemService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- IHoldItemService implementation --

        public IEnumerable<HoldItemModel> GetHoldItemsByTableNumber(int tableNumber)
        {
            var result = _allHoldItems;

            if (tableNumber != 0)
            {
                result = _allHoldItems.Where(x => x.TableNumber == tableNumber);
            }

            return result;
        }

        public IEnumerable<HoldItemBindableModel> GetSortedHoldItems(EHoldItemsSortingType typeSort, IEnumerable<HoldItemBindableModel> holdItems)
        {
            Func<HoldItemBindableModel, object> sortingSelector = typeSort switch
            {
                EHoldItemsSortingType.ByTableName => x => x.TableNumber,
                EHoldItemsSortingType.ByItem => x => x.Item,
                _ => throw new NotImplementedException(),
            };

            return holdItems.OrderBy(sortingSelector);
        }

        public async Task<AOResult<IEnumerable<HoldItemModel>>> GetAllHoldItemsAsync()
        {
            var result = new AOResult<IEnumerable<HoldItemModel>>();

            try
            {
                _allHoldItems = await _mockService.GetAllAsync<HoldItemModel>();

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

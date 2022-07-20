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
        private readonly IMapper _mapper;

        private IEnumerable<HoldItemBindableModel> _holdItems = Enumerable.Empty<HoldItemBindableModel>();

        public HoldItemService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- IHoldItemService implementation --

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
                var allHoldItems = await _mockService.GetAllAsync<HoldItemModel>();

                if (allHoldItems is not null)
                {
                    result.SetSuccess(allHoldItems);
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

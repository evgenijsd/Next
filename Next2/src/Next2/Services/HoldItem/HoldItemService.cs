using AutoMapper;
using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
using Next2.Services.Messages;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.HoldItem
{
    public class HoldItemService : IHoldItemService
    {
        private readonly IMockService _mockService;
        private readonly IMapper _mapper;
        private readonly IMessagesService _messagesService;

        private IEnumerable<HoldItemBindableModel> _holdItems = Enumerable.Empty<HoldItemBindableModel>();

        public HoldItemService(
            IMockService mockService,
            IMessagesService messagesService,
            IMapper mapper)
        {
            _mockService = mockService;
            _mapper = mapper;
            _messagesService = messagesService;
        }

        #region -- IHoldItemService implementation --

        public async Task<IEnumerable<HoldItemBindableModel>> GetHoldItems()
        {
            var holdItems = await GetAllHoldItemsAsync();

            if (holdItems.IsSuccess)
            {
                _holdItems = _mapper.Map<List<HoldItemBindableModel>>(holdItems.Result);
                await _messagesService.ResponseToBadRequestAsync(Constants.StatusCode.SOCKET_CLOSED);
            }
            else
            {
                await _messagesService.ResponseToBadRequestAsync(holdItems.Exception.Message);
            }

            return _holdItems;
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

        #endregion

        #region -- Private helpers --

        private async Task<AOResult<IEnumerable<HoldItemModel>>> GetAllHoldItemsAsync()
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

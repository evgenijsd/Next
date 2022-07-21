using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.Bindables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.HoldItem
{
    public interface IHoldItemService
    {
        Task<AOResult<IEnumerable<HoldItemModel>>> GetAllHoldItemsAsync();

        IEnumerable<HoldItemBindableModel> GetSortedHoldItems(EHoldItemsSortingType typeSort, IEnumerable<HoldItemBindableModel> reservations);

        IEnumerable<HoldItemModel> GetHoldItemsByTableNumber(int tableNumber);
    }
}

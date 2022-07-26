using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.Bindables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.OrdersHolding
{
    public interface IOrdersHolding
    {
        Task<AOResult<IEnumerable<HoldItemModel>>> GetAllHoldItemsAsync();

        IEnumerable<HoldItemBindableModel> GetSortedHoldItems(EHoldItemsSortingType typeSort, IEnumerable<HoldItemBindableModel> holdDihes);

        IEnumerable<HoldItemModel> GetHoldItemsByTableNumber(int tableNumber);
    }
}

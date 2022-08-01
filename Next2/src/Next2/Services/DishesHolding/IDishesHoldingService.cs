using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.Bindables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.DishesHolding
{
    public interface IDishesHoldingService
    {
        Task<AOResult<IEnumerable<HoldDishModel>>> GetAllHoldDishesAsync();

        IEnumerable<HoldDishBindableModel> GetSortedHoldDishes(EHoldDishesSortingType typeSort, IEnumerable<HoldDishBindableModel> holdDihes);

        IEnumerable<HoldDishModel> GetHoldDishesByTableNumber(int tableNumber);
    }
}

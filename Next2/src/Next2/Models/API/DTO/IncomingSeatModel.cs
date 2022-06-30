using System.Collections.Generic;

namespace Next2.Models.API.DTO
{
    public class IncomingSeatModel
    {
        public int Number { get; set; }

        public IEnumerable<IncomingSelectedDishModel> SelectedDishes { get; set; }
    }
}

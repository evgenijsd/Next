using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.Api.DTO
{
    public class IncomingSeatModel
    {
        public int Number { get; set; }
        public IEnumerable<IncomingSelectedDishModel> SelectedDishes { get; set; }
    }
}

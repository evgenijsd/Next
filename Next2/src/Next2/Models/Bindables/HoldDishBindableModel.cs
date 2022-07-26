using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models.Bindables
{
    public class HoldDishBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public int TableNumber { get; set; }

        public string DishName { get; set; } = string.Empty;

        public DateTime ReleaseTime { get; set; }
    }
}

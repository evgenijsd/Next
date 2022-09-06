using Next2.Interfaces;
using Prism.Mvvm;
using System;

namespace Next2.Models.Bindables
{
    public class HoldDishBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid DishId { get; set; }

        public int TableNumber { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime ReleaseTime { get; set; }
    }
}

using Next2.Interfaces;
using Prism.Mvvm;
using System;

namespace Next2.Models.Bindables
{
    public class ProportionBindableModel : BindableBase, IBaseApiModel
    {
        public Guid Id { get; set; }

        public Guid ProportionId { get; set; }

        public decimal PriceRatio { get; set; }

        public string? ProportionName { get; set; }

        public decimal Price { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}

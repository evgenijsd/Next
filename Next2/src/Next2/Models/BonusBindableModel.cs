using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.API.DTO;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Next2.Models
{
    public class BonusBindableModel : BindableBase, IBaseApiModel, ITappable
    {
        public Guid Id { get; set; }

        public string? Name { get; set; } = string.Empty;

        public int DiscountPercentage { get; set; }

        public bool IsActive { get; set; }

        public IEnumerable<SimpleDishModelDTO>? Dishes { get; set; }

        public ICommand? TapCommand { get; set; }

        public EBonusType Type { get; set; }

        public decimal Value { get; set; }

        public EBonusValueType BonusValueType { get; set; }
    }
}

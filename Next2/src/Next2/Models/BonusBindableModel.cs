using Next2.ENums;
using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class BonusBindableModel : BindableBase, IBaseModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Value { get; set; }

        public EBonusType Type { get; set; }
    }
}

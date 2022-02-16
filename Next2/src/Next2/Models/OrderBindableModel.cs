using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Models
{
    public class OrderBindableModel : BindableBase
    {
        #region -- Public properties --

        public int TableNumber { get; set; }

        public int OrderNumber { get; set; }

        public string Name { get; set; }

        public string? OrderStatus { get; set; }

        public string? OrderType { get; set; }

        public string OrderNumberText { get; set; }

        public double Total { get; set; }

        #endregion
    }
}

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

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string? _orderStatus;
        public string? OrderStatus
        {
            get => _orderStatus;
            set => SetProperty(ref _orderStatus, value);
        }

        private string? _orderType;
        public string? OrderType
        {
            get => _orderType;
            set => SetProperty(ref _orderType, value);
        }

        private int _orderNumber;
        public int OrderNumber
        {
            get => _orderNumber;
            set => SetProperty(ref _orderNumber, value);
        }

        private double _total;
        public double Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        #endregion
    }
}

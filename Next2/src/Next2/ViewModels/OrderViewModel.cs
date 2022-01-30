using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.ViewModels
{
    public class OrderViewModel : BindableBase
    {
        #region -- Public properties --

        public int TableNumber { get; set; }

        private bool _isSelect;

        public bool IsSelect
        {
            get => _isSelect;
            set => SetProperty(ref _isSelect, value);
        }

        private string _Name;

        public string Name
        {
            get => _Name;
            set => SetProperty(ref _Name, value);
        }

        private string _orderStatus;

        public string OrderStatus
        {
            get => _orderStatus;
            set => SetProperty(ref _orderStatus, value);
        }

        private string _orderType;

        public string OrderType
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

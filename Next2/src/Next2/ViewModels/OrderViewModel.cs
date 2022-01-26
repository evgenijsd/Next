using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.ViewModels
{
    public class OrderViewModel : BindableBase
    {
        private string _customerName;

        public string CustomerName
        {
            get => _customerName;
            set => SetProperty(ref _customerName, value);
        }

        private string _tableName;

        public string TableName
        {
            get => _tableName;
            set => SetProperty(ref _tableName, value);
        }

        private int _orderNumber;

        public int OrderNumber
        {
            get => _orderNumber;
            set => SetProperty(ref _orderNumber, value);
        }

        private int _total;

        public int Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }
    }
}

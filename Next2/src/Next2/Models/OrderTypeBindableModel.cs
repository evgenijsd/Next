using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;

namespace Next2.Models
{
    public class OrderTypeBindableModel : BindableBase
    {
        public EOrderType OrderType { get; set; }

        public string OrderTypeValue { get; set; }
    }
}

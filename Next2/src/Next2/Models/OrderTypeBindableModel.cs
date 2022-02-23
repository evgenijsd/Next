using Next2.Enums;
using Prism.Mvvm;

namespace Next2.Models
{
    public class OrderTypeBindableModel : BindableBase
    {
        public EOrderType OrderType { get; set; }

        public string Text { get; set; }
    }
}

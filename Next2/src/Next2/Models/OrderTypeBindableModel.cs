using Next2.Enums;
using Prism.Mvvm;
using System.Collections.Generic;

namespace Next2.Models
{
    public class OrderTypeBindableModel : BindableBase
    {
        public KeyValuePair<EOrderType, string> KeyValuePair { get; set; }
    }
}

using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Windows.Input;

namespace Next2.Helpers
{
    public class PaymentItem : BindableBase, ITappable
    {
        public EPaymentItems PayemenType { get; set; }

        public string Text { get; set; }

        public ICommand TapCommand { get; set; }
    }
}

using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Windows.Input;

namespace Next2.Helpers
{
    public class PaymentItem : BindableBase, ITappable
    {
        public EPaymentItems PaymentType { get; set; }

        public string Text { get; set; } = string.Empty;

        public ICommand? TapCommand { get; set; }
    }
}

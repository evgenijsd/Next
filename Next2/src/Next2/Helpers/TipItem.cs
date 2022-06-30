using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Windows.Input;

namespace Next2.Helpers
{
    public class TipItem : BindableBase, ITappable
    {
        public ETipType TipType { get; set; }

        public string Text { get; set; } = string.Empty;

        public decimal PercentTip { get; set; }

        public decimal Value { get; set; }

        public ICommand TapCommand { get; set; }
    }
}

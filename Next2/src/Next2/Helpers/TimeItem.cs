using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System.Windows.Input;

namespace Next2.Helpers
{
    public class TimeItem : BindableBase, ITappable
    {
        public int Minute { get; set; }

        public bool IsSelected { get; set; }

        public ICommand? TapCommand { get; set; }
    }
}

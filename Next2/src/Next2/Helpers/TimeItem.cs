using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Next2.Helpers
{
    public class TimeItem : BindableBase, ITappable
    {
        public int Minutes { get; set; }

        public ICommand TapCommand { get; set; }
    }
}

using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Next2.Helpers
{
    public class TimeItem : BindableBase
    {
        public int Minute { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Next2.Interfaces
{
    public interface ITappable
    {
        ICommand TapCommand { get; set; }
    }
}

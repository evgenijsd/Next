using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Next2.Controls.StateContainer
{
    [ContentProperty("Content")]
    public class StackContainerCondition : ContentView
    {
        public object? State { get; set; }

        public object? NotState { get; set; }
    }
}

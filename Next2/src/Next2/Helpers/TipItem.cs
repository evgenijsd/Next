using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Next2.Helpers
{
    public class TipItem : BindableBase, ITappable
    {
        public string Text { get; set; } = string.Empty;

        public float PercentTip { get; set; }

        public float Value { get; set; } = 0f;

        public ICommand TapCommand { get; set; }
    }
}

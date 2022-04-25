using Next2.Enums;
using Next2.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Next2.Helpers
{
    public class TipsItem : BindableBase, ITappable
    {
        public ETipsItems TipsType { get; set; }

        public string Text { get; set; }

        public float PercentTips { get; set; }

        public ICommand TapCommand { get; set; }
    }
}

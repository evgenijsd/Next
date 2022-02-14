using System;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class CustomDateTimePanelTemplate : ContentView
    {
        public CustomDateTimePanelTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public DateTime CurrentDateTime { get; set; } = DateTime.Now;

        #endregion
    }
}
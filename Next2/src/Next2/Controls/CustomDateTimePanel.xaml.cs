using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class CustomDateTimePanel : ContentView
    {
        public CustomDateTimePanel()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public DateTime CurrentDateTime { get; set; } = DateTime.Now;

        #endregion
    }
}
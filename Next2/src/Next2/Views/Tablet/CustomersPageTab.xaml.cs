using Next2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Views.Tablet
{
    public partial class CustomersPageTab : BaseContentPage
    {
        public CustomersPageTab()
        {
            InitializeComponent();
        }

        private void ItemTappedHandler(object sender, EventArgs args)
        {
            selectButton.Opacity = 1;
            infoButton.Opacity = 1;
        }
    }
}
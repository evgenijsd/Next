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
        private ViewCell lastCell;
        public CustomersPageTab()
        {
            InitializeComponent();
        }

        private void ViewCellTappedHandler(object sender, EventArgs args)
        {
            if (lastCell != null)
            {
                lastCell.View.BackgroundColor = Color.Transparent;
            }

            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                viewCell.View.BackgroundColor = Color.FromHex("#AB3821");
                lastCell = viewCell;
            }

            selectButton.Opacity = 1;
            infoButton.Opacity = 1;
        }
    }
}
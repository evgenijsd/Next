using System;
using Xamarin.Forms;

namespace Next2.Views.Mobile
{
    public partial class OrderTabsPage : BaseContentPage
    {
        public OrderTabsPage()
        {
            InitializeComponent();
        }

        private void StackButton_SizeChanged(object sender, EventArgs e)
        {
            collectionData.ScrollTo(14, position: ScrollToPosition.Center);
        }
    }
}
using System;

using Xamarin.Forms;

namespace Next2.Views.Mobile
{
    public partial class OrderTabPage : BaseContentPage
    {
        public OrderTabPage()
        {
            InitializeComponent();
        }

        private void StackButton_SizeChanged(object sender, EventArgs e)
        {
            collectionData.ScrollTo(14, position: ScrollToPosition.Center);
        }
    }
}
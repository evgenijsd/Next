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

        private void collectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (collectionView?.SelectedItem != null)
            {
                collectionView.ScrollTo(collectionView.SelectedItem, position: ScrollToPosition.Center, animate: false);
            }
        }

        private void collectionView_SizeChanged(object sender, EventArgs e)
        {
            var x = collectionView.Height;
            var y = view.Height;
            int i = 0;
        }
    }
}
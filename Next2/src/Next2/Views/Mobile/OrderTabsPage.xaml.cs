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
            if (collectionView != null && collectionView.SelectedItem != null)
            {
                collectionView.ScrollTo(collectionView.SelectedItem, position: ScrollToPosition.MakeVisible);
            }
        }
    }
}
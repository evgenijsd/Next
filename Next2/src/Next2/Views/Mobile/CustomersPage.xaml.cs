using Xamarin.Forms;

namespace Next2.Views.Mobile
{
    public partial class CustomersPage : BaseContentPage
    {
        private double _viewItemHeight;

        public CustomersPage()
        {
            InitializeComponent();
        }

        public int IndexLastVisibleElement { get; set; }

        private void collectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (_viewItemHeight == 0f && collectionView.ItemTemplate.CreateContent() is View view)
            {
                _viewItemHeight = view.HeightRequest;
            }

            if (_viewItemHeight > 0)
            {
                IndexLastVisibleElement = (int)((collectionView.Height + e.VerticalOffset) / _viewItemHeight);
            }
        }
    }
}
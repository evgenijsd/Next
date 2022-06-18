using Xamarin.Forms;

namespace Next2.Views.Mobile
{
    public partial class CustomersPage : BaseContentPage
    {
        public CustomersPage()
        {
            InitializeComponent();
        }

        public int IndexLastVisibleElement { get; set; }

        private void collectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            IndexLastVisibleElement = e.LastVisibleItemIndex + 1;
        }
    }
}
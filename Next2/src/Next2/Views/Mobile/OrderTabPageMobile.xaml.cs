using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Views
{
    public partial class OrderTabPageMobile : BaseContentPage
    {
        public OrderTabPageMobile()
        {
            InitializeComponent();
        }

        private void StackButton_SizeChanged(object sender, EventArgs e)
        {
            collectionData.ScrollTo(14, position: ScrollToPosition.Center);
        }
    }
}
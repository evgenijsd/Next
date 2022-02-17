using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Views.Mobile
{
    public partial class SearchPage : BaseContentPage
    {
        public SearchPage()
        {
            InitializeComponent();
        }

        private void BaseContentPage_Appearing(object sender, EventArgs e)
        {
            EntryLocal.Focus();
        }
    }
}
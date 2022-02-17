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
        private bool _isEntryFocusOn = true;

        public SearchPage()
        {
            InitializeComponent();
        }

        private void BaseContentPage_Appearing(object sender, EventArgs e)
        {
            EntryLocal.Focus();
        }

        private void BaseContentPage_Disappearing(object sender, EventArgs e)
        {
            _isEntryFocusOn = false;

            EntryLocal.Unfocus();
            EntryLocal.IsEnabled = false;
        }

        private void EntryLocal_Unfocused(object sender, FocusEventArgs e)
        {
            if (_isEntryFocusOn)
            {
                //EntryLocal.Focus();
            }
        }
    }
}
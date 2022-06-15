using System;

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
            searchEntry.Focus();
        }
    }
}
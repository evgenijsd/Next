using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Views
{
    public partial class AddCommentPage : BaseContentPage
    {
        public AddCommentPage()
        {
            InitializeComponent();
        }

        #region -- Overrides --

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (string.IsNullOrWhiteSpace(customEntry.Text))
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await System.Threading.Tasks.Task.Delay(250);
                    customEntry.Focus();
                });
            }
        }

        #endregion
    }
}
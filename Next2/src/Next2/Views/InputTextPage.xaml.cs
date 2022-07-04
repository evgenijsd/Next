using Xamarin.Forms;

namespace Next2.Views
{
    public partial class InputTextPage : BaseContentPage
    {
        public InputTextPage()
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
using Xamarin.Forms;

namespace Next2.Views.Mobile
{
    public partial class LoginPage_EmployeeId : BaseContentPage
    {
        public LoginPage_EmployeeId()
        {
            InitializeComponent();
        }

        #region -- Overrides --

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Device.BeginInvokeOnMainThread(async () =>
            {
                await System.Threading.Tasks.Task.Delay(250);
                customEntry.Focus();
            });
        }

        #endregion

    }
}
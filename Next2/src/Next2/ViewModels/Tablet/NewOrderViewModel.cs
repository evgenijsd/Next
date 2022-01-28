using Prism.Navigation;

namespace Next2.ViewModels.Tablet
{
    public class NewOrderViewModel : BaseViewModel
    {
        public NewOrderViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "NewOrder";
        }

        #region -- Public properties --

        public string? Text { get; set; }

        #endregion
    }
}
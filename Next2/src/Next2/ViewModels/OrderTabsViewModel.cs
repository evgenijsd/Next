using Prism.Navigation;

namespace Next2.ViewModels
{
    public class OrderTabsViewModel : BaseViewModel
    {
        public OrderTabsViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "OrderTabs";
        }

        #region -- Public properties --

        public string? Text { get; set; }

        #endregion
    }
}

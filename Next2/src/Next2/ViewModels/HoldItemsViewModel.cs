using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;

namespace Next2.ViewModels
{
    public class HoldItemsViewModel : BaseViewModel
    {
        public HoldItemsViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation)
            : base(navigationService, popupNavigation)
        {
            Text = "HoldItems";
        }

        #region -- Public properties --

        public string? Text { get; set; }

        #endregion
    }
}

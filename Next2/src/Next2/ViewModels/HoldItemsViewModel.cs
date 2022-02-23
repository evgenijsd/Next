using Next2.Views.Mobile;
using Next2.Views.Tablet;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class HoldItemsViewModel : BaseViewModel
    {
        public HoldItemsViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "HoldItems";
        }

        #region -- Public properties --

        public string? Text { get; set; }

        private ICommand _goToEmployeeIdPage;
        public ICommand GoToEmployeeIdPage => _goToEmployeeIdPage ??= new AsyncCommand(OnGoToEmployeeIdPageAsync);

        private async Task OnGoToEmployeeIdPageAsync()
        {
            string page = App.IsTablet ? nameof(HoldItemsView) : nameof(HoldItemsPage);
            var parameters = new NavigationParameters { { Constants.Navigations.ADMIN, page } };
            await _navigationService.NavigateAsync(nameof(Views.Mobile.LoginPage), parameters);
        }

        #endregion
    }
}

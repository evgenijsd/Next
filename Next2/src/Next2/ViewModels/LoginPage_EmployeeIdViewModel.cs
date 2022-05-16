using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class LoginPage_EmployeeIdViewModel : BaseViewModel
    {
        public LoginPage_EmployeeIdViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation)
          : base(navigationService, popupNavigation)
        {
        }

        #region -- Public properties--

        public string EmployeeId { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        #endregion

        #region -- Private helpers --

        private async Task OnGoBackCommandAsync()
        {
            var navigationParameters = new NavigationParameters
            {
                { nameof(Constants.Navigations.REFRESH_ORDER), Constants.Navigations.REFRESH_ORDER },
            };

            if (!App.IsTablet)
            {
                EmployeeId = new string(EmployeeId?.Where(char.IsDigit).ToArray());

                navigationParameters.Add(Constants.Navigations.EMPLOYEE_ID, EmployeeId);
            }

            await _navigationService.GoBackAsync(navigationParameters);
        }

        #endregion
    }
}
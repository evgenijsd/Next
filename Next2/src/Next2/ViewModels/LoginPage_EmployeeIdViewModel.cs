using Next2.Models;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class LoginPage_EmployeeIdViewModel : BaseViewModel
    {
        public LoginPage_EmployeeIdViewModel(INavigationService navigationService)
          : base(navigationService)
        {
        }

        #region -- Public properties--

        public string EmployeeId { get; set; }

        public SetBindableModel? SelectedSet { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.TryGetValue(Constants.Navigations.SELECTED_SET, out SetBindableModel set))
            {
                SelectedSet = set;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnGoBackCommandAsync()
        {
            if (!App.IsTablet)
            {
                EmployeeId = new string(EmployeeId?.Where(char.IsDigit).ToArray());

                var navigationParameters = new NavigationParameters
                {
                    { Constants.Navigations.EMPLOYEE_ID, EmployeeId },
                    { nameof(Constants.Navigations.REFRESH_ORDER), Constants.Navigations.REFRESH_ORDER },
                };
                await _navigationService.GoBackAsync(navigationParameters);
            }
            else
            {
                var navigationParameters = new NavigationParameters
                {
                    { nameof(Constants.Navigations.REFRESH_ORDER), Constants.Navigations.REFRESH_ORDER },
                };
                await _navigationService.GoBackAsync(navigationParameters);
            }
        }

        #endregion
    }
}
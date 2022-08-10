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

        #region -- Public properties --

        public string EmployeeId { get; set; } = string.Empty;

        private ICommand? _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        #endregion

        #region -- Private helpers --

        private Task OnGoBackCommandAsync()
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

            return _navigationService.GoBackAsync(navigationParameters);
        }

        #endregion
    }
}
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Mobile
{
    public class LoginPage_EmployeeIdViewModel : BaseViewModel
    {
        public LoginPage_EmployeeIdViewModel(INavigationService navigationService)
          : base(navigationService)
        {
        }

          #region -- Public properties--

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        private string _employeeId;
        public string EmployeeId
        {
            get => _employeeId;
            set => SetProperty(ref _employeeId, value);
        }

        #endregion

        #region -- Private helpers --
        private async Task OnGoBackCommandAsync()
        {
            var navigationParameters = new NavigationParameters
            {
                 { nameof(EmployeeId), EmployeeId },
            };

            await _navigationService.GoBackAsync(navigationParameters);
        }

        #endregion
    }
}

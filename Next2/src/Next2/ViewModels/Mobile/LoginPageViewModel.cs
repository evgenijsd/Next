using Next2.Views.Mobile;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class LoginPageViewModel : BaseViewModel
    {
        public LoginPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }
        #region -- Public properties--

        private ICommand _ButtonClearCommand;
        public ICommand ButtonClearCommand => _ButtonClearCommand ??= new AsyncCommand(OnTabClearAsync);

        private ICommand _goToEmployeeIdPage;
        public ICommand GoToEmployeeIdPage => _goToEmployeeIdPage ??= new AsyncCommand(OnGoToEmployeeIdPageAsync);

        public DateTime CurrentDate = DateTime.Now;

        private string _employeeId = "Type Employee ID";
        public string EmployeeId
        {
            get => _employeeId;
            set => SetProperty(ref _employeeId, value);
        }

        #endregion

        #region -- Private helpers --
        private async Task OnTabClearAsync()
        {
            EmployeeId = string.Empty;
        }

        private async Task OnGoToEmployeeIdPageAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(LoginPage_EmployeeId)}");
        }
        #endregion

        #region -- Overrides --
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            _employeeId = string.Empty;
           parameters.TryGetValue("EmployeeId", out _employeeId);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        #endregion

    }
}

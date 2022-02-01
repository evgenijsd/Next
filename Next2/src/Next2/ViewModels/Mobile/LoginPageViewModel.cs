using Next2.Views.Mobile;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private string _inputtedEmployeeId;

        #region -- Public properties--

        private ICommand _ButtonClearCommand;
        public ICommand ButtonClearCommand => _ButtonClearCommand ??= new AsyncCommand(OnTabClearAsync);

        private ICommand _goToEmployeeIdPage;
        public ICommand GoToEmployeeIdPage => _goToEmployeeIdPage ??= new AsyncCommand(OnGoToEmployeeIdPageAsync);

        public DateTime CurrentDate { get; set; } = DateTime.Now;

        public bool IsErrorStrokeVisible { get; set; } = true;

        public bool IsButtonEnable { get; set; } = false;

        public string EmployeeId { get; set; } = "Type Employee ID";

        #endregion

        #region -- Private helpers --

        private async Task OnTabClearAsync()
        {
            EmployeeId = "Type Employee ID";

            IsErrorStrokeVisible = false;
        }

        private async Task OnGoToEmployeeIdPageAsync()
        {
            await _navigationService.NavigateAsync(nameof(LoginPage_EmployeeId));
        }

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue("EmployeeId", out _inputtedEmployeeId))
            {
                EmployeeId = !string.IsNullOrWhiteSpace(_inputtedEmployeeId) ? EmployeeId = _inputtedEmployeeId : EmployeeId = EmployeeId;
            }
        }

        #endregion

    }
}

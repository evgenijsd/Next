using Next2.Views;
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

        private int _inputtedEmployeeIdToDigist;

        private int _correctEmployeeId = 2020;

        #region -- Public properties--

        public bool IsEmployeeExists { get; set; }

        private ICommand _ButtonClearCommand;
        public ICommand ButtonClearCommand => _ButtonClearCommand ??= new AsyncCommand(OnTabClearAsync);

        private ICommand _goToStartPageCommand;
        public ICommand GoToStartPageCommand => _goToStartPageCommand ??= new AsyncCommand<object>(OnStartPageCommandAsync);

        private ICommand _goToEmployeeIdPage;
        public ICommand GoToEmployeeIdPage => _goToEmployeeIdPage ??= new AsyncCommand(OnGoToEmployeeIdPageAsync);

        public DateTime CurrentDate { get; set; } = DateTime.Now;

        public bool IsErrorStrokeVisible { get; set; }

        public bool IsClearButtonEnabled { get; set; }

        public bool IsLoginButtonEnabled { get; set; }

        public string EmployeeId { get; set; } = "Type Employee ID";

        #endregion

        #region -- Private helpers --

        private async Task OnTabClearAsync()
        {
            EmployeeId = "Type Employee ID";

            IsErrorStrokeVisible = false;

            IsClearButtonEnabled = false;

            IsLoginButtonEnabled = false;
        }

        private async Task OnGoToEmployeeIdPageAsync()
        {
            await _navigationService.NavigateAsync(nameof(LoginPage_EmployeeId));
        }

        private async Task OnStartPageCommandAsync(object? sender)
        {
            var label = sender as Xamarin.Forms.Label;

            if (label is not null)
            {
                int.TryParse(label.Text, out _inputtedEmployeeIdToDigist);

                IsEmployeeExists = _inputtedEmployeeIdToDigist == _correctEmployeeId;

                if (IsEmployeeExists)
                {
                    await _navigationService.NavigateAsync(nameof(StartPage));
                }
                else
                {
                    IsErrorStrokeVisible = true;
                }
            }
        }

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue("EmployeeId", out _inputtedEmployeeId))
            {
                IsClearButtonEnabled = !string.IsNullOrWhiteSpace(_inputtedEmployeeId);

                EmployeeId = !string.IsNullOrWhiteSpace(_inputtedEmployeeId) ? EmployeeId = _inputtedEmployeeId : EmployeeId = EmployeeId;

                int.TryParse(_inputtedEmployeeId, out _inputtedEmployeeIdToDigist);

                IsErrorStrokeVisible = (_inputtedEmployeeIdToDigist != _correctEmployeeId) && !string.IsNullOrWhiteSpace(_inputtedEmployeeId);

                IsEmployeeExists = (_inputtedEmployeeIdToDigist != _correctEmployeeId) ? false : true;

                IsLoginButtonEnabled = IsEmployeeExists;
            }
        }

        #endregion

    }
}

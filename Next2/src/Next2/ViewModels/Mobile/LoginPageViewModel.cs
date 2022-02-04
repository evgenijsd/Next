using Next2.Services;
using Next2.Services.Authentication;
using Next2.Views;
using Next2.Views.Mobile;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class LoginPageViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly IMockService _mockService;

        private string _inputtedEmployeeId;

        private int _inputtedEmployeeIdToDigist;

        public LoginPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            IMockService mockService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _mockService = mockService;
        }

        #region -- Public properties--

        public bool IsEmployeeExists { get; set; }

        public bool IsErrorNotificationVisible { get; set; }

        public bool IsClearButtonEnabled { get; set; }

        public bool IsLoginButtonEnabled { get; set; }

        public DateTime CurrentDate { get; set; } = DateTime.Now;

        public string EmployeeId { get; set; } = "Type Employee ID";

        private ICommand _ButtonClearCommand;
        public ICommand ButtonClearCommand => _ButtonClearCommand ??= new AsyncCommand(OnTabClearAsync);

        private ICommand _goToStartPageCommand;
        public ICommand GoToStartPageCommand => _goToStartPageCommand ??= new AsyncCommand<object>(OnStartPageCommandAsync);

        private ICommand _goToEmployeeIdPage;
        public ICommand GoToEmployeeIdPage => _goToEmployeeIdPage ??= new AsyncCommand(OnGoToEmployeeIdPageAsync);

        #endregion

        #region -- Private helpers --

        private async Task CheckEmployeeExists()
        {
            IsEmployeeExists = IsLoginButtonEnabled = (await _authenticationService.AuthorizationAsync(_inputtedEmployeeIdToDigist)).IsSuccess;
        }

        private async Task OnTabClearAsync()
        {
            EmployeeId = "Type Employee ID";

            IsErrorNotificationVisible = IsClearButtonEnabled = IsLoginButtonEnabled = false;
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

                await CheckEmployeeExists();

                if (IsEmployeeExists)
                {
                    await _navigationService.NavigateAsync(nameof(StartPage));
                }
                else
                {
                    IsErrorNotificationVisible = true;
                }
            }
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            parameters.TryGetValue("EmployeeId", out _inputtedEmployeeId);

            if (!string.IsNullOrWhiteSpace(_inputtedEmployeeId))
            {
                IsEmployeeExists = IsLoginButtonEnabled = false;

                IsClearButtonEnabled = !string.IsNullOrWhiteSpace(_inputtedEmployeeId);

                EmployeeId = !string.IsNullOrWhiteSpace(_inputtedEmployeeId) ? _inputtedEmployeeId : EmployeeId;

                int.TryParse(_inputtedEmployeeId, out _inputtedEmployeeIdToDigist);

                await CheckEmployeeExists();

                IsErrorNotificationVisible = !IsEmployeeExists && !string.IsNullOrWhiteSpace(_inputtedEmployeeId);
            }
        }

        #endregion

    }
}

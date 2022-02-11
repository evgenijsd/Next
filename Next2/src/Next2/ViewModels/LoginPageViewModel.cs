using Next2.Services;
using Next2.Services.Authentication;
using Next2.Views.Mobile;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
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

        public DateTime CurrentDate { get; set; } = DateTime.Now;

        public string EmployeeId { get; set; } = LocalizationResourceManager.Current["TypeEmployeeId"];

        private ICommand _ButtonClearCommand;
        public ICommand ButtonClearCommand => _ButtonClearCommand ??= new AsyncCommand(OnTabClearAsync);

        private ICommand _goToStartPageCommand;
        public ICommand GoToStartPageCommand => _goToStartPageCommand ??= new AsyncCommand<object>(OnStartPageCommandAsync);

        private ICommand _goToEmployeeIdPage;
        public ICommand GoToEmployeeIdPage => _goToEmployeeIdPage ??= new AsyncCommand(OnGoToEmployeeIdPageAsync);

        #endregion

        #region -- Private helpers --
        private async Task OnTabClearAsync()
        {
            EmployeeId = LocalizationResourceManager.Current["TypeEmployeeId"];
        }

        private async Task OnGoToEmployeeIdPageAsync()
        {
            await _navigationService.NavigateAsync(nameof(LoginPage_EmployeeId));
        }

        private async Task OnStartPageCommandAsync(object? sender)
        {
            if (sender is string str && str is not null)
            {
                if (str.Length == Constants.LOGIN_PASSWORD_LENGTH)
                {
                    int.TryParse(str, out _inputtedEmployeeIdToDigist);

                    await CheckEmployeeExists();

                    if (IsEmployeeExists)
                    {
                        await _navigationService.NavigateAsync($"{nameof(Views.Tablet.MenuPage)}");
                    }
                    else
                    {
                        IsErrorNotificationVisible = true;
                    }
                }
                else
                {
                    IsErrorNotificationVisible = true;
                }
            }
            else if (IsEmployeeExists)
            {
                await _navigationService.NavigateAsync($"{nameof(MenuPage)}");
            }
        }

        private async Task CheckEmployeeExists()
        {
            IsEmployeeExists = (await _authenticationService.AuthorizeAsync(_inputtedEmployeeIdToDigist)).IsSuccess;
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue("EmployeeId", out _inputtedEmployeeId))
            {
                if (!string.IsNullOrWhiteSpace(_inputtedEmployeeId) && _inputtedEmployeeId.Length == Constants.LOGIN_PASSWORD_LENGTH)
                {
                    int.TryParse(_inputtedEmployeeId, out _inputtedEmployeeIdToDigist);
                    await CheckEmployeeExists();
                    EmployeeId = _inputtedEmployeeId;
                }
                else if (!string.IsNullOrWhiteSpace(_inputtedEmployeeId) && _inputtedEmployeeId.Length != Constants.LOGIN_PASSWORD_LENGTH)
                {
                    IsEmployeeExists = false;
                    EmployeeId = _inputtedEmployeeId;
                }
            }
        }

        #endregion

    }
}

using Next2.Enums;
using Next2.Helpers;
using Next2.Services.Authentication;
using Next2.Services.UserService;
using Next2.Views.Mobile;
using Prism.Events;
using Prism.Navigation;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEventAggregator _eventAggregator;

        public LoginPageViewModel(
            INavigationService navigationService,
            IUserService userService,
            IAuthenticationService authenticationService,
            IEventAggregator eventAggregator)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _eventAggregator = eventAggregator;
        }

        #region -- Public properties--

        public bool IsUserLogIn { get; set; }

        public bool IsErrorNotificationVisible { get; set; }

        public string EmployeeId { get; set; } = string.Empty;

        public DateTime CurrentDateTime { get; set; }

        private ICommand _ClearCommand;
        public ICommand ClearCommand => _ClearCommand ??= new AsyncCommand(OnClearCommandAsync);

        private ICommand _goToStartPageCommand;
        public ICommand GoToStartPageCommand => _goToStartPageCommand ??= new AsyncCommand(OnStartPageCommandAsync);

        private ICommand _goToEmployeeIdPage;
        public ICommand GoToEmployeeIdPage => _goToEmployeeIdPage ??= new AsyncCommand(OnGoToEmployeeIdPageAsync);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (App.IsTablet && args.PropertyName == nameof(EmployeeId))
            {
                IsErrorNotificationVisible = false;
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (_authenticationService.IsAuthorizationComplete)
            {
                await _navigationService.NavigateAsync($"{nameof(MenuPage)}");
            }
            else if (parameters.TryGetValue(Constants.Navigations.EMPLOYEE_ID, out string inputtedEmployeeId))
            {
                CheckEmployeeId(inputtedEmployeeId);

                EmployeeId = inputtedEmployeeId;
            }
            else if (parameters.TryGetValue(Constants.Navigations.RESULT, out bool isUserLoggedOut))
            {
                await OnClearCommandAsync();
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            OnClearCommandAsync().ConfigureAwait(false);
        }

        #endregion

        #region -- Private helpers --

        private Task OnClearCommandAsync()
        {
            EmployeeId = string.Empty;
            IsErrorNotificationVisible = false;

            return Task.CompletedTask;
        }

        private async Task OnGoToEmployeeIdPageAsync()
        {
            await _navigationService.NavigateAsync(nameof(LoginPage_EmployeeId));
        }

        private async Task OnStartPageCommandAsync()
        {
            if (App.IsTablet)
            {
                CheckEmployeeId(EmployeeId);
            }

            await AuithorizationUserAsync();
        }

        private async Task AuithorizationUserAsync()
        {
            if (!IsErrorNotificationVisible && EmployeeId != string.Empty)
            {
                var result = await _authenticationService.AuthorizationUserAsync(EmployeeId);

                if (result.IsSuccess)
                {
                    await _navigationService.NavigateAsync($"{nameof(Views.Tablet.MenuPage)}");

                    IsUserLogIn = true;
                }
            }
        }

        private void CheckEmployeeId(string inputtedValue)
        {
            var result = !string.IsNullOrWhiteSpace(inputtedValue) && inputtedValue.Length == Constants.Limits.LOGIN_LENGTH;

            IsErrorNotificationVisible = !result && inputtedValue != string.Empty;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CurrentDateTime = DateTime.Now;
        }

        #endregion
    }
}

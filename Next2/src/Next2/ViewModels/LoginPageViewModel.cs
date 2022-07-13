using Next2.Services.Authentication;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Events;
using Prism.Navigation;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IOrderService _orderService;
        private readonly IEventAggregator _eventAggregator;

        public LoginPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            IOrderService orderService,
            IEventAggregator eventAggregator)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _orderService = orderService;
            _eventAggregator = eventAggregator;
        }

        #region -- Public properties --

        public bool IsActivityIndicatorRunning { get; set; }

        public bool IsUserLogIn { get; set; }

        public bool IsInvalidEmployeeId { get; set; }

        public string EmployeeId { get; set; } = string.Empty;

        private ICommand _ClearCommand;
        public ICommand ClearCommand => _ClearCommand ??= new AsyncCommand(OnClearCommandAsync);

        private ICommand _goToStartPageCommand;
        public ICommand GoToStartPageCommand => _goToStartPageCommand ??= new AsyncCommand(OnGoToStartPageCommandAsync);

        private ICommand _goToEmployeeIdPageCommand;
        public ICommand GoToEmployeeIdPageCommand => _goToEmployeeIdPageCommand ??= new AsyncCommand(OnGoToEmployeeIdPageCommandAsync);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(EmployeeId))
            {
                IsInvalidEmployeeId = false;
            }
        }

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            await base.InitializeAsync(parameters);

            if (_authenticationService.IsAuthorizationComplete)
            {
                await _orderService.OpenLastOrCreateNewOrderAsync();

                await _navigationService.NavigateAsync($"{nameof(MenuPage)}");
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.EMPLOYEE_ID, out string inputtedEmployeeId))
            {
                CheckEmployeeId(inputtedEmployeeId);

                EmployeeId = inputtedEmployeeId;
            }
            else if (parameters.TryGetValue(Constants.Navigations.LOGOUT, out bool isUserLoggedOut))
            {
                await OnClearCommandAsync();
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            OnClearCommandAsync();
        }

        #endregion

        #region -- Private helpers --

        private Task OnClearCommandAsync()
        {
            EmployeeId = string.Empty;

            return Task.CompletedTask;
        }

        private Task OnGoToEmployeeIdPageCommandAsync()
        {
            return _navigationService.NavigateAsync(nameof(LoginPage_EmployeeId));
        }

        private Task OnGoToStartPageCommandAsync()
        {
            if (App.IsTablet)
            {
                CheckEmployeeId(EmployeeId);
            }

            return AuthorizeUserAsync();
        }

        private async Task AuthorizeUserAsync()
        {
            if (!IsInvalidEmployeeId && EmployeeId != string.Empty)
            {
                IsActivityIndicatorRunning = true;

                var result = await _authenticationService.AuthorizeUserAsync(EmployeeId);

                if (result.IsSuccess)
                {
                    await _orderService.OpenLastOrCreateNewOrderAsync();

                    IsActivityIndicatorRunning = false;

                    await _navigationService.NavigateAsync($"{nameof(MenuPage)}");

                    IsUserLogIn = true;
                }
                else
                {
                    IsInvalidEmployeeId = true;
                    IsActivityIndicatorRunning = false;
                }
            }
        }

        private void CheckEmployeeId(string inputtedValue)
        {
            var isEmployeeIdValid = !string.IsNullOrWhiteSpace(inputtedValue) && inputtedValue.Length == Constants.Limits.LOGIN_LENGTH;

            IsInvalidEmployeeId = !isEmployeeIdValid && inputtedValue != string.Empty;
        }

        #endregion
    }
}

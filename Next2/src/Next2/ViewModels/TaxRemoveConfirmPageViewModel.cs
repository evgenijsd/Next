using Next2.Services.Authentication;
using Next2.Services.Notifications;
using Next2.Views.Mobile;
using Prism.Events;
using Prism.Navigation;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class TaxRemoveConfirmPageViewModel : BaseViewModel
    {
        private readonly IEventAggregator _eventAggregator;

        public TaxRemoveConfirmPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            IEventAggregator eventAggregator)
            : base(navigationService, authenticationService, notificationsService)
        {
            _eventAggregator = eventAggregator;
        }

        #region -- Public properties --

        public bool IsAdminAccount { get; set; }

        public bool IsInvalidEmployeeId { get; set; }

        public string EmployeeId { get; set; } = string.Empty;

        private ICommand? _clearCommand;
        public ICommand ClearCommand => _clearCommand ??= new AsyncCommand(OnClearCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _removeTaxCommand;
        public ICommand RemoveTaxCommand => _removeTaxCommand ??= new AsyncCommand(OnRemoveTaxCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _openEmployeeIdInputPageCommand;
        public ICommand OpenEmployeeIdInputPageCommand => _openEmployeeIdInputPageCommand ??= new AsyncCommand(OnOpenEmployeeIdInputPageCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (App.IsTablet && args.PropertyName == nameof(EmployeeId))
            {
                IsInvalidEmployeeId = false;
            }
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.EMPLOYEE_ID, out string inputtedEmployeeId))
            {
                await CheckEmployeeId(inputtedEmployeeId);

                EmployeeId = inputtedEmployeeId;
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
            IsAdminAccount = false;
            IsInvalidEmployeeId = false;

            return Task.CompletedTask;
        }

        private Task OnOpenEmployeeIdInputPageCommandAsync()
        {
            return _navigationService.NavigateAsync(nameof(LoginPage_EmployeeId));
        }

        private async Task OnRemoveTaxCommandAsync()
        {
            if (App.IsTablet)
            {
                await CheckEmployeeId(EmployeeId);
            }

            if (IsAdminAccount)
            {
                var parameters = new NavigationParameters { { Constants.Navigations.TAX_OFF, IsAdminAccount } };

                await _navigationService.GoBackAsync(parameters);
            }
        }

        private async Task CheckEmployeeId(string employeeId)
        {
            var isAdminAccount = false;

            if (employeeId.Length == Constants.Limits.LOGIN_LENGTH)
            {
                var resultOfGettingUser = await _authenticationService.GetUserById(employeeId);

                if (resultOfGettingUser.IsSuccess)
                {
                    var roles = resultOfGettingUser.Result?.Roles;

                    if (roles is not null)
                    {
                        isAdminAccount = roles.Contains(Constants.ROLE_ADMIN);
                    }
                }
                else
                {
                    await ResponseToUnsuccessfulRequestAsync(resultOfGettingUser.Exception?.Message);
                }
            }

            IsAdminAccount = isAdminAccount;
            IsInvalidEmployeeId = !isAdminAccount && employeeId != string.Empty;
        }

        #endregion
    }
}

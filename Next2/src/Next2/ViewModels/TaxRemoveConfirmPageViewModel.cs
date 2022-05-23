﻿using Next2.Enums;
using Next2.Helpers.Events;
using Next2.Services.Authentication;
using Next2.Services.UserService;
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
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEventAggregator _eventAggregator;

        public TaxRemoveConfirmPageViewModel(
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

        public bool IsAdminAccount { get; set; }

        public bool IsInvalidEmployeeId { get; set; }

        public string EmployeeId { get; set; } = string.Empty;

        private ICommand _ClearCommand;
        public ICommand ClearCommand => _ClearCommand ??= new AsyncCommand(OnClearCommandAsync, allowsMultipleExecutions: false);

        private ICommand _RemoveTaxCommand;
        public ICommand RemoveTaxCommand => _RemoveTaxCommand ??= new AsyncCommand(OnRemoveTaxCommandAsync, allowsMultipleExecutions: false);

        private ICommand _openEmployeeIdInputPageCommand;
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
                _eventAggregator.GetEvent<TaxRemovedEvent>().Publish(!IsAdminAccount);
                await _navigationService.GoBackAsync();
            }
        }

        private async Task CheckEmployeeId(string inputtedValue)
        {
            var isAdminAccount = false;

            if ((inputtedValue.Length == Constants.Limits.LOGIN_LENGTH)
                && int.TryParse(inputtedValue, out int inputtedEmployeeIdToDigit))
            {
                var user = await _authenticationService.CheckUserExists(inputtedEmployeeIdToDigit);

                if (user.IsSuccess)
                {
                    isAdminAccount = user.Result.UserType == EUserType.Admin;
                }
            }

            IsAdminAccount = isAdminAccount;
            IsInvalidEmployeeId = !isAdminAccount && inputtedValue != string.Empty;
        }

        #endregion
    }
}
﻿using Next2.Services;
using Next2.Services.Authentication;
using Next2.Services.MockService;
using Next2.Services.UserService;
using Next2.Views.Mobile;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class LoginPageViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authenticationService;

        private string _inputtedEmployeeId;

        private int _inputtedEmployeeIdToDigit;

        public LoginPageViewModel(
            INavigationService navigationService,
            IUserService userService,
            IAuthenticationService authenticationService)
            : base(navigationService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
        }

        #region -- Public properties--

        public bool IsEmployeeExists { get; set; }

        public bool IsCheckAdminID { get; set; } = false;

        public bool IsUserLogIn { get; set; }

        public bool IsErrorNotificationVisible { get; set; }

        public DateTime CurrentDateTime { get; set; }

        public string EmployeeId { get; set; } = string.Empty;

        private ICommand _buttonClearCommand;
        public ICommand ButtonClearCommand => _buttonClearCommand ??= new AsyncCommand(OnTabClearOrCancelAsync);

        private ICommand _goToStartPageCommand;
        public ICommand GoToStartPageCommand => _goToStartPageCommand ??= new AsyncCommand<object>(OnStartPageOrConfirmCommandAsync);

        private ICommand _goToEmployeeIdPage;
        public ICommand GoToEmployeeIdPage => _goToEmployeeIdPage ??= new AsyncCommand(OnGoToEmployeeIdPageAsync);

        #endregion

        #region -- Private helpers --

        private async Task OnTabClearOrCancelAsync()
        {
            if (IsCheckAdminID)
            {
                await _navigationService.GoBackAsync();
            }
            else
            {
                EmployeeId = string.Empty;
                IsEmployeeExists = false;
            }
        }

        private async Task OnGoToEmployeeIdPageAsync()
        {
            await _navigationService.NavigateAsync(nameof(LoginPage_EmployeeId));
        }

        private async Task OnStartPageOrConfirmCommandAsync(object? sender)
        {
            if (sender is string str && str is not null)
            {
                if (str.Length == Constants.LOGIN_PASSWORD_LENGTH)
                {
                    int.TryParse(str, out _inputtedEmployeeIdToDigit);

                    await CheckEmployeeExists();

                    if (IsEmployeeExists)
                    {
                        _authenticationService.Authorization();

                        if (IsCheckAdminID)
                        {
                            await _navigationService.GoBackAsync();
                        }
                        else
                        {
                            await _navigationService.NavigateAsync($"{nameof(Views.Tablet.MenuPage)}");
                        }

                        IsUserLogIn = true;
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
                if (IsCheckAdminID)
                {
                    await _navigationService.GoBackAsync();
                }
                else
                {
                    _authenticationService.Authorization();
                    await _navigationService.NavigateAsync($"{nameof(MenuPage)}");
                }
            }
        }

        private async Task CheckEmployeeExists()
        {
            IsEmployeeExists = (await _authenticationService.CheckUserExists(_inputtedEmployeeIdToDigit)).IsSuccess;
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.ADMIN, out string page))
            {
                IsCheckAdminID = true;
            }

            if (_authenticationService.AuthorizedUserId >= 0 && !IsCheckAdminID)
            {
                await _navigationService.NavigateAsync($"{nameof(MenuPage)}");
            }
            else
            {
                if (parameters.TryGetValue("EmployeeId", out _inputtedEmployeeId) && !string.IsNullOrWhiteSpace(_inputtedEmployeeId))
                {
                    if (_inputtedEmployeeId.Length == Constants.LOGIN_PASSWORD_LENGTH && int.TryParse(_inputtedEmployeeId, out _inputtedEmployeeIdToDigit))
                    {
                        await CheckEmployeeExists();
                        EmployeeId = _inputtedEmployeeId;
                    }
                    else
                    {
                        IsEmployeeExists = false;
                        EmployeeId = _inputtedEmployeeId;
                    }
                }
                else if (parameters.TryGetValue("result", out bool isUserLoggedOut))
                {
                    IsUserLogIn = !IsUserLogIn;
                    IsEmployeeExists = false;
                }
            }
        }

        public override void OnAppearing()
        {
            base.OnAppearing();
            var timerUpdateTime = new Timer(1);
            timerUpdateTime.Elapsed += Timer_Elapsed;

            Task.Run(() => timerUpdateTime.Start());
        }

        #endregion

        #region -- Private helpers --

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CurrentDateTime = DateTime.Now;
        }

        #endregion

    }
}

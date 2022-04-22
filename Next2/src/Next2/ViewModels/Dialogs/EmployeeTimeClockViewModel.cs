using Next2.Enums;
using Next2.Services.Authentication;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Dialogs
{
    public class EmployeeTimeClockViewModel : BindableBase
    {
        private readonly IAuthenticationService _authenticationService;
        public EmployeeTimeClockViewModel(
            IAuthenticationService authenticationService,
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            _authenticationService = authenticationService;
            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(null));
            AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true } }));
            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));
            State = EEmployeeRegisterState.Undefinite;
            ScreenKeyboard = string.Empty;
        }

        #region --Public Properties--

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        public DelegateCommand AcceptCommand { get; set; }

        public DelegateCommand DeclineCommand { get; }

        public string ScreenKeyboard { get; set; }

        public EEmployeeRegisterState State { get; set; }

        public bool IsErrorNotificationVisible { get; set; }

        public DateTime DateTime { get; set; }

        private ICommand _applyButtonCommand;
        public ICommand ApplyButtonCommand => _applyButtonCommand ??= new AsyncCommand(OnApplyButtonCommand, allowsMultipleExecutions: false);

        private ICommand _cancelButtonCommand;
        public ICommand CancelButtonCommand => _cancelButtonCommand ??= new AsyncCommand(OnCancelButtonCommand, allowsMultipleExecutions: false);

        #endregion

        #region --Private Helpers--

        private async Task OnApplyButtonCommand()
        {
            if (ScreenKeyboard.Length != 6)
            {
                IsErrorNotificationVisible = true;
            }
            else if (int.TryParse(ScreenKeyboard, out int employeeId))
            {
                var result = await _authenticationService.CheckUserExists(employeeId);

                if (result.IsSuccess)
                {
                    // Code will be here
                    State = EEmployeeRegisterState.CheckedIn;
                    DateTime = DateTime.Now;
                }
            }
            else
            {
                IsErrorNotificationVisible = true;
            }
        }

        private Task OnCancelButtonCommand()
        {
            CloseCommand.Execute();
            return Task.CompletedTask;
        }

        #endregion

    }
}

using Next2.Enums;
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
        public EmployeeTimeClockViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
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

        private Task OnApplyButtonCommand()
        {
            DateTime = DateTime.Now;
            State = EEmployeeRegisterState.CheckedIn;
          //  AcceptCommand.Execute();
            return Task.CompletedTask;
        }

        private Task OnCancelButtonCommand()
        {
            CloseCommand.Execute();
            return Task.CompletedTask;
        }

        #endregion

    }
}

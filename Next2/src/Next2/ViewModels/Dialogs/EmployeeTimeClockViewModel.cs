using Next2.Enums;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Services.WorkLog;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Prism.Xaml;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Dialogs
{
    public class EmployeeTimeClockViewModel : BindableBase
    {
        private readonly IWorkLogService _workLogService;

        public EmployeeTimeClockViewModel(
            IWorkLogService workLogService,
            Action<IDialogParameters> requestClose)
        {
            _workLogService = workLogService;

            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(new DialogParameters()));
        }

        #region -- Public properties --

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        public string EmployeeId { get; set; } = string.Empty;

        public EEmployeeRegisterState State { get; set; } = EEmployeeRegisterState.Undefined;

        public bool IsErrorNotificationVisible { get; set; }

        public DateTime DateTime { get; set; }

        private ICommand? _applyCommand;
        public ICommand ApplyCommand => _applyCommand ??= new AsyncCommand(OnApplyCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new AsyncCommand(OnCancelCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private async Task OnApplyCommandAsync()
        {
            if (EmployeeId.Length != Constants.Limits.LOGIN_LENGTH || !int.TryParse(EmployeeId, out int employeeId))
            {
                IsErrorNotificationVisible = true;
            }
            else
            {
                var resultOfLoggingWorkTime = await _workLogService.LogWorkTimeAsync(EmployeeId);

                if (resultOfLoggingWorkTime.IsSuccess)
                {
                    var timeTrack = resultOfLoggingWorkTime.Result;

                    (State, DateTime) = timeTrack.Start is null
                        ? (EEmployeeRegisterState.Undefined, DateTime.MinValue)
                        : timeTrack.End is not null
                            ? (EEmployeeRegisterState.CheckedOut, (DateTime)timeTrack.End)
                            : (EEmployeeRegisterState.CheckedIn, (DateTime)timeTrack.Start);
                }
                else
                {
                    var parameters = new DialogParameters() { { Constants.DialogParameterKeys.NEED_TO_OFFER_LOGOUT, true }, };

                    RequestClose(parameters);
                }
            }
        }

        private Task OnCancelCommandAsync()
        {
            CloseCommand.Execute();
            return Task.CompletedTask;
        }

        #endregion
    }
}

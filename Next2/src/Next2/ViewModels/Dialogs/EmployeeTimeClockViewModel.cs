using Next2.Enums;
using Next2.Models;
using Next2.Services.Log;
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
        private readonly ILogService _logServise;
        public EmployeeTimeClockViewModel(
            ILogService logService,
            Action<IDialogParameters> requestClose)
        {
            _logServise = logService;
            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(null));
            State = EEmployeeRegisterState.Undefined;
            EmployeeId = string.Empty;
        }

        #region --Public Properties--

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        public string EmployeeId { get; set; }

        public EEmployeeRegisterState State { get; set; }

        public bool IsErrorNotificationVisible { get; set; }

        public DateTime DateTime { get; set; }

        private ICommand _applyCommand;
        public ICommand ApplyCommand => _applyCommand ??= new AsyncCommand(OnApplyCommandAsync, allowsMultipleExecutions: false);

        private ICommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand ??= new AsyncCommand(OnCancelCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region --Private Helpers--

        private async Task OnApplyCommandAsync()
        {
            if (EmployeeId.Length != Constants.Limits.EMPLOYEE_ID_LENGTH || !int.TryParse(EmployeeId, out int employeeId))
            {
                IsErrorNotificationVisible = true;
            }
            else
            {
                var record = new WorkLogRecordModel
                {
                    Timestamp = DateTime.Now,
                    EmployeeId = employeeId,
                };
                var state = await _logServise.InsertRecordAsync(record);

                if (state.IsSuccess)
                {
                    DateTime = record.Timestamp;
                    State = state.Result;
                }
                else
                {
                    IsErrorNotificationVisible = true;
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

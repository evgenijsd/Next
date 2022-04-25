﻿using Next2.Enums;
using Next2.Models;
using Next2.Services.Authentication;
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
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            _logServise = logService;
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
                var record = new WorkLogRecordModel
                {
                    Timestamp = DateTime.Now,
                    EmployeeId = employeeId,
                };
                var state = await _logServise.InsertRecord(record);
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

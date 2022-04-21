﻿using Next2.Enums;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

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

        #endregion

    }
}

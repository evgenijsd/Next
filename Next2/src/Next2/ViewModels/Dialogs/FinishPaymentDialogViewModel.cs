﻿using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class FinishPaymentDialogViewModel : BindableBase
    {
        public FinishPaymentDialogViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            OkCommand = new Command(() => RequestClose(null));
        }

        #region -- Public properties --

        public Action<IDialogParameters> RequestClose;

        public ICommand OkCommand { get; }

        #endregion
    }
}

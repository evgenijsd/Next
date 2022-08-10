﻿using Next2.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Next2.ViewModels.Dialogs
{
    public class CustomerInfoViewModel : BindableBase
    {
        public CustomerInfoViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            SetupParameters(param);

            RequestClose = requestClose;

            CloseCommand = new DelegateCommand(() => RequestClose(new DialogParameters()));
            AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true } }));
            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));
        }

        #region -- Public properties --

        public string CancelButtonText { get; set; } = string.Empty;

        public string SelectButtonText { get; set; } = string.Empty;

        public CustomerBindableModel Customer { get; set; } = new();

        #endregion

        public DelegateCommand CloseCommand { get; }

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand AcceptCommand { get; }

        public DelegateCommand DeclineCommand { get; }

        #region -- Private helpers --

        private void SetupParameters(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.MODEL, out CustomerBindableModel customer))
            {
                Customer = customer;
            }

            if (param.TryGetValue(Constants.DialogParameterKeys.OK_BUTTON_TEXT, out string buttonText))
            {
                SelectButtonText = buttonText;
            }

            if (param.TryGetValue(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, out string cancelText))
            {
                CancelButtonText = cancelText;
            }
        }

        #endregion
    }
}

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.ViewModels.Dialogs
{
    public class LogOutAlertViewModel : BindableBase
    {
        public LogOutAlertViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(null));
            AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true } }));
            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));
        }

        #region -- Public properties --

        public string Title { get; set; }
        public string OkButtonText { get; set; }

        public string CancelButtonText { get; set; }

        public DelegateCommand CloseCommand { get; }
        public DelegateCommand AcceptCommand { get; }
        public DelegateCommand DeclineCommand { get; }

        #endregion

        public Action<IDialogParameters> RequestClose;
    }
}

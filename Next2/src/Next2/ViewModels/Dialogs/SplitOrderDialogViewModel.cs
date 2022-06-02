using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
     public class SplitOrderDialogViewModel
    {
        public SplitOrderDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            AcceptCommand = new Command(() => RequestClose(null));
            DeclineCommand = new Command(() => RequestClose(null));
        }

        #region -- Public Properties --

        public Action<IDialogParameters> RequestClose;

        public ICommand DeclineCommand { get; }

        public ICommand AcceptCommand { get; }

        #endregion

    }
}

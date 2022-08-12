using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Next2.ViewModels.Dialogs
{
    public class ProgrammDeviceDialogViewModel : BindableBase
    {
        public ProgrammDeviceDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;

            InitParameters(param);
        }

        #region -- Public Properties --

        public Action<IDialogParameters>? RequestClose;

        #endregion

        #region -- Private Helpers --

        private void InitParameters(IDialogParameters param)
        {
        }

        #endregion
    }
}

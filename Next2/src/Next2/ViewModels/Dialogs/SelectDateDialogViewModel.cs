using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.ViewModels.Dialogs
{
    public class SelectDateDialogViewModel : BindableBase
    {
        public SelectDateDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            LoadData(param);
        }

        #region -- Public Properties --

        public Action<IDialogParameters> RequestClose;

        #endregion

        #region -- Private Helpers --

        private void LoadData(DialogParameters param)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

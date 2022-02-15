using Next2.Models;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.ViewModels.Tablet.Dialogs
{
    public class AddSetToOrderDialogViewModel
    {
        public AddSetToOrderDialogViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(null));

            if (param.ContainsKey(Constants.DialogParameterKeys.SET))
            {
                SetModel set;

                if (param.TryGetValue(Constants.DialogParameterKeys.SET, out set))
                {
                    Set = set;
                }
            }
        }

        #region --Public Properties--

        public SetModel Set { get; }

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        #endregion
    }
}
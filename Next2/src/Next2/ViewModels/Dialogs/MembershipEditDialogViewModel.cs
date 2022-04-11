using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.ViewModels.Dialogs
{
    public class MembershipEditDialogViewModel : BindableBase
    {
        public MembershipEditDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(null));
        }

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }
    }
}

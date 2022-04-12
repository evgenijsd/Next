using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class MembershipEditDialogViewModel : BindableBase
    {
        public MembershipEditDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            RequestClose = requestClose;
            CloseCommand = new Command(() => RequestClose(null));
            DisableMembershipCommand = new Command(OnDisableMembershipCommand);
            SaveMembershipCommand = new Command(OnSaveMembershipCommand);
        }

        public Action<IDialogParameters> RequestClose;

        public ICommand CloseCommand { get; }

        public ICommand DisableMembershipCommand { get; }

        public ICommand SaveMembershipCommand { get; }

        private void OnDisableMembershipCommand()
        {
            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.ACCEPT, true } };

            RequestClose(dialogParameters);
        }

        private void OnSaveMembershipCommand()
        {
            var dialogParameters = new DialogParameters { { Constants.DialogParameterKeys.ACCEPT, true } };

            RequestClose(dialogParameters);
        }
    }
}

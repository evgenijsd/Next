using Next2.Models;
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
            CloseCommand = new DelegateCommand(() => RequestClose(null));
            AcceptCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, true } }));
            DeclineCommand = new DelegateCommand(() => RequestClose(new DialogParameters() { { Constants.DialogParameterKeys.ACCEPT, false } }));
        }

        #region --Public Properties--

        public string CancelButtonText { get; set; }

        public string SelectButtonText { get; set; }

        public CustomerBindableModel Customer { get; set; }

        #endregion

        public DelegateCommand CloseCommand { get; }

        public Action<IDialogParameters> RequestClose;
        public DelegateCommand AcceptCommand { get; }
        public DelegateCommand DeclineCommand { get; }

        #region --Private Helpers--

        private void SetupParameters(IDialogParameters param)
        {
            if (param.ContainsKey(Constants.DialogParameterKeys.MODEL))
            {
                CustomerBindableModel customer;
                param.TryGetValue(Constants.DialogParameterKeys.MODEL, out customer);
                Customer = customer;
            }

            if (param.ContainsKey(Constants.DialogParameterKeys.OK_BUTTON_TEXT))
            {
                string s;
                param.TryGetValue(Constants.DialogParameterKeys.OK_BUTTON_TEXT, out s);
                SelectButtonText = s;
            }

            if (param.ContainsKey(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT))
            {
                string s;
                param.TryGetValue(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, out s);
                CancelButtonText = s;
            }
        }

        #endregion
    }
}

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;

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

        private string _cancelButtonText;
        public string CancelButtonText
        {
            get => _cancelButtonText;
            set => SetProperty(ref _cancelButtonText, value);
        }

        private string _selectButtonText;
        public string SelectButtonText
        {
            get => _selectButtonText;
            set => SetProperty(ref _selectButtonText, value);
        }

        private CustomersViewModel _customer;
        public CustomersViewModel Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }
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
                param.TryGetValue(Constants.DialogParameterKeys.MODEL, out _customer);
            }

            if (param.ContainsKey(Constants.DialogParameterKeys.OK_BUTTON_TEXT))
            {
                param.TryGetValue(Constants.DialogParameterKeys.OK_BUTTON_TEXT, out _selectButtonText);
            }

            if (param.ContainsKey(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT))
            {
                param.TryGetValue(Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, out _cancelButtonText);
            }
        }

        #endregion
    }
}

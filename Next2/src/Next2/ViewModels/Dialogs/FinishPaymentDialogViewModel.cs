using Next2.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class FinishPaymentDialogViewModel : BindableBase
    {
        public FinishPaymentDialogViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            SetupParameters(param);
            RequestClose = requestClose;
            EmailCommand = new DelegateCommand(() => RequestClose(new DialogParameters { { Constants.DialogParameterKeys.SEND_EMAIL, true } }));
            TextCommand = new DelegateCommand(() => RequestClose(new DialogParameters { { Constants.DialogParameterKeys.TEXT, true } }));
            PrintCommand = new DelegateCommand(() => RequestClose(new DialogParameters { { Constants.DialogParameterKeys.PRINT, true } }));
            NoReceiptCommand = new DelegateCommand(() => requestClose(new DialogParameters { { Constants.DialogParameterKeys.NO_RECEIPT, true } }));
        }

        #region -- Public properties --

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand EmailCommand { get; }

        public DelegateCommand TextCommand { get; }

        public DelegateCommand PrintCommand { get; }

        public DelegateCommand NoReceiptCommand { get; }

        public PaidOrderBindableModel Order { get; set; }

        #endregion

        #region -- Private Helpers --

        private void SetupParameters(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.PAID_ORDER_BINDABLE_MODEL, out PaidOrderBindableModel order))
            {
                Order = order;
            }
        }

        #endregion

    }
}

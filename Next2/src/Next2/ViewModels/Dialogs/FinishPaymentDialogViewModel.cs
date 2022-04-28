using Next2.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Dialogs
{
    public class FinishPaymentDialogViewModel : BindableBase
    {
        public FinishPaymentDialogViewModel(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            SetupParameters(param);
            RequestClose = requestClose;
            CloseCommand = new DelegateCommand(() => RequestClose(new DialogParameters { { Constants.DialogParameterKeys.PAYMENT_COMPLETE, true } }));
        }

        #region -- Public properties --

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; }

        private ICommand _emailCommand;
        public ICommand EmailCommand => _emailCommand ??= new AsyncCommand(OnEmailCommandAsync, allowsMultipleExecutions: false);

        private ICommand _textCommand;
        public ICommand TextCommand => _textCommand ??= new AsyncCommand(OnTextCommandAsync, allowsMultipleExecutions: false);

        private ICommand _printCommand;
        public ICommand PrintCommand => _printCommand ??= new AsyncCommand(OnPrintCommandAsync, allowsMultipleExecutions: false);

        private ICommand _noReceiptCommand;
        public ICommand NoReceiptCommand => _noReceiptCommand ??= new AsyncCommand(NoReceiptCommandAsync, allowsMultipleExecutions: false);

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

        private Task OnEmailCommandAsync()
        {
            throw new NotImplementedException();
        }

        private Task OnTextCommandAsync()
        {
            throw new NotImplementedException();
        }

        private Task OnPrintCommandAsync()
        {
            throw new NotImplementedException();
        }

        private Task NoReceiptCommandAsync()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

using Next2.Enums;
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
        public FinishPaymentDialogViewModel(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            SetupParameters(param);
            RequestClose = requestClose;
        }

        #region -- Public properties --

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand CloseCommand { get; set; }

        public PaidOrderBindableModel Order { get; set; }

        private ICommand _finishPaymentCommand;
        public ICommand FinishPaymentCommand => _finishPaymentCommand ??= new AsyncCommand<EPaymentReceiptOptions>(OnFinishPaymentCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private Helpers --

        private void SetupParameters(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.PAID_ORDER_BINDABLE_MODEL, out PaidOrderBindableModel order))
            {
                Order = order;
            }
        }

        private Task OnFinishPaymentCommandAsync(EPaymentReceiptOptions receiptOptions)
        {
            var param = new DialogParameters
            {
                { Constants.DialogParameterKeys.PAYMENT_COMPLETE, receiptOptions },
            };

            CloseCommand = new DelegateCommand(() => RequestClose(param));
            CloseCommand.Execute();

            return Task.CompletedTask;
        }

        #endregion

    }
}

using Next2.Enums;
using Next2.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Dialogs
{
    public class FinishPaymentDialogViewModel : BindableBase
    {
        public FinishPaymentDialogViewModel(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            SetupParameters(parameters);

            RequestClose = requestClose;
        }

        #region -- Public properties --

        public Action<IDialogParameters> RequestClose;

        public DelegateCommand? CloseCommand { get; set; }

        public PaidOrderBindableModel Order { get; set; } = new();

        public EBonusType BonusType { get; set; }

        public string TipValue { get; set; } = string.Empty;

        private ICommand? _finishPaymentCommand;
        public ICommand FinishPaymentCommand => _finishPaymentCommand ??= new AsyncCommand<EPaymentReceiptOptions>(OnFinishPaymentCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private void SetupParameters(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.PAID_ORDER_BINDABLE_MODEL, out PaidOrderBindableModel order))
            {
                Order = order;
                BonusType = Order.BonusType;
            }

            if (parameters.TryGetValue(Constants.DialogParameterKeys.TIP_VALUE_DIALOG, out string tipValue))
            {
                TipValue = tipValue;
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

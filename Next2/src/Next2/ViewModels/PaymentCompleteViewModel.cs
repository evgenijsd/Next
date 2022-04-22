using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Views.Mobile;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class PaymentCompleteViewModel : BaseViewModel
    {
        private ICommand _tapPaymentOptionCommand;

        public PaymentCompleteViewModel(
            INavigationService navigationService,
            PaidOrderBindableModel order)
            : base(navigationService)
        {
            Order = order;

            _tapPaymentOptionCommand = new AsyncCommand<PaymentItem>(OnTapPaymentOptionCommandAsync, allowsMultipleExecutions: false);

            Task.Run(InitPaymentOptionsAsync);
        }

        #region -- Public properties --

        public ObservableCollection<PaymentItem> PaymentOptionsItems { get; set; } = new();

        public PaymentItem SelectedPaymentOption { get; set; } = new();

        public PaidOrderBindableModel Order { get; set; }

        public bool IsExpandedSummary { get; set; } = true;

        #endregion

        #region -- Private helpers --

        private Task InitPaymentOptionsAsync()
        {
            PaymentOptionsItems = new()
            {
                new()
                {
                    PayemenType = EPaymentItems.Tips,
                    Text = "Tips",
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PayemenType = EPaymentItems.GiftCards,
                    Text = "Gift Cards",
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PayemenType = EPaymentItems.Cash,
                    Text = "Cash",
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PayemenType = EPaymentItems.Card,
                    Text = "Card",
                    TapCommand = _tapPaymentOptionCommand,
                },
            };

            SelectedPaymentOption = PaymentOptionsItems[3];

            return Task.CompletedTask;
        }

        private async Task OnTapPaymentOptionCommandAsync(PaymentItem item)
        {
            string path = string.Empty;

            switch (item.PayemenType)
            {
                case EPaymentItems.Cash:
                    path = nameof(InputCashPage);
                    break;
                case EPaymentItems.Card:
                    path = nameof(WaitingSwipeCardPage);
                    break;
            }

            await _navigationService.NavigateAsync(path);
        }

        #endregion
    }
}

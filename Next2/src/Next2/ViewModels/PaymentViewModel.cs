using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Customers;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Services.Rewards;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly INotificationsService _notificationsService;

        private decimal _subtotalWithBonus;

        public PaymentViewModel(
            INavigationService navigationService,
            IMapper mapper,
            IOrderService orderService,
            ICustomersService customerService,
            INotificationsService notificationsService,
            IRewardsService rewardsService)
            : base(navigationService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _notificationsService = notificationsService;

            if (_orderService.CurrentOrder.Discount is null && _orderService.CurrentOrder.Coupon is null)
            {
                Order.BonusType = EBonusType.None;
            }
            else
            {
                Order.BonusType = _orderService.CurrentOrder.Coupon is null
                    ? EBonusType.Discount
                    : EBonusType.Coupon;

                Order.Bonus = _orderService.CurrentOrder.Coupon is null
                    ? _mapper.Map<BonusBindableModel>(_orderService.CurrentOrder.Discount)
                    : _mapper.Map<BonusBindableModel>(_orderService.CurrentOrder.Coupon);
            }

            Order.SubtotalWithBonus = _orderService.CurrentOrder.DiscountPrice == null
                ? 0
                : (decimal)_orderService.CurrentOrder.DiscountPrice;

            Order.Subtotal = _orderService.CurrentOrder.SubTotalPrice == null
                ? 0
                : (decimal)_orderService.CurrentOrder.SubTotalPrice;

            Order.PriceTax = orderService.CurrentOrder.PriceTax;
            Order.TaxCoefficient = orderService.CurrentOrder.TaxCoefficient;
            Order.Total = orderService.CurrentOrder.TotalPrice;
            Order.Customer = orderService.CurrentOrder.Customer;
            _subtotalWithBonus = Order.BonusType == EBonusType.None
                ? Order.Subtotal
                : Order.SubtotalWithBonus;

            if (Order.Customer is not null && Order.Customer.GiftCards.Any())
            {
                Order.GiftCardsTotalFunds = Order.Customer.GiftCardsTotalFund;
                Order.RemainingGiftCardsTotalFunds = Order.GiftCardsTotalFunds;
            }

            RewardsViewModel = new(
                navigationService,
                mapper,
                orderService,
                customerService,
                rewardsService,
                Order,
                notificationsService,
                NavigateAsync,
                GoToPaymentStep);

            PaymentCompleteViewModel = new(
                navigationService,
                customerService,
                orderService,
                mapper,
                notificationsService,
                Order);
        }

        #region -- Public properties --

        public PaidOrderBindableModel Order { get; set; } = new();

        public EPaymentSteps PaymentStep { get; set; }

        public RewardsViewModel RewardsViewModel { get; set; }

        public PaymentCompleteViewModel PaymentCompleteViewModel { get; set; }

        private ICommand _backCancelCommand;
        public ICommand BackCancelCommand => _backCancelCommand ??= new AsyncCommand(OnBackCancelCommandAsync, allowsMultipleExecutions: false);

        private ICommand _OpenTipsCommand;
        public ICommand OpenTipsCommand => _OpenTipsCommand ??= new AsyncCommand(OnOpenTipsCommandAsync, allowsMultipleExecutions: false);

        private Task OnOpenTipsCommandAsync()
        {
            return _navigationService.NavigateAsync(nameof(TipsPage));
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.TIP_VALUE, out TipItem tipItem))
            {
                Order.Tip = tipItem.Value;
                PaymentCompleteViewModel.SelectedTipItem = tipItem;

                PaymentCompleteViewModel.RecalculateTotal();
            }
            else if (parameters.TryGetValue(Constants.Navigations.INPUT_VALUE, out string inputValue))
            {
                if (decimal.TryParse(inputValue, out decimal sum))
                {
                    Order.Cash = sum / 100;
                }
            }
            else if (parameters.TryGetValue(Constants.Navigations.GIFT_CARD_FOUNDS, out string inputAmountValue))
            {
                if (parameters.ContainsKey(Constants.Navigations.GIFT_CARD_ADDED)
                    && _orderService.CurrentOrder.Customer is not null)
                {
                    _orderService.CurrentOrder.Customer.IsUpdatedCustomer = false;

                    Order.Subtotal = _orderService.CurrentOrder.SubTotalPrice == null
                        ? 0
                        : (decimal)_orderService.CurrentOrder.SubTotalPrice;

                    Order.PriceTax = _orderService.CurrentOrder.PriceTax;
                    Order.Total = _orderService.CurrentOrder.TotalPrice;
                    Order.GiftCardsTotalFunds = Order.Customer.GiftCardsTotalFund;
                    Order.RemainingGiftCardsTotalFunds = Order.GiftCardsTotalFunds;
                }

                if (Order.Customer is not null && Order.Customer.GiftCards.Any())
                {
                    Order.Total += Order.GiftCard;
                    Order.GiftCard = 0;

                    if (decimal.TryParse(inputAmountValue, out decimal sum))
                    {
                        sum /= 100;

                        if (Order.GiftCardsTotalFunds >= sum)
                        {
                            if (Order.Total > sum)
                            {
                                Order.GiftCard = sum;
                                Order.RemainingGiftCardsTotalFunds -= sum;
                                Order.Total -= sum;
                            }
                            else
                            {
                                Order.RemainingGiftCardsTotalFunds = Order.GiftCardsTotalFunds - Order.Total;
                                Order.GiftCard = Order.Total;
                                Order.Total = 0;
                            }
                        }
                    }
                }
            }
            else if (parameters.ContainsKey(Constants.Navigations.PAYMENT_COMPLETE))
            {
                PaymentCompleteViewModel.IsPaymentComplete = true;
            }
            else
            {
                RewardsViewModel.OnNavigatedTo(parameters);
            }
        }

        #endregion

        #region -- Private helpers --

        private async void NavigateAsync(NavigationMessage navigationMessage) =>
            await _navigationService.NavigateAsync(navigationMessage.Path, navigationMessage.Parameters);

        private void GoToPaymentStep(EPaymentSteps step) => PaymentStep = step;

        private async Task OnBackCancelCommandAsync()
        {
            if (PaymentStep == EPaymentSteps.Complete)
            {
                PaymentStep = EPaymentSteps.Rewards;
            }
            else
            {
                if (RewardsViewModel.Order.IsUnsavedChangesExist)
                {
                    var confirmDialogParameters = new DialogParameters
                    {
                        { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                        { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                        { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["PaymentNotSavedMessage"] },
                        { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Ok"] },
                    };

                    PopupPage confirmDialog = App.IsTablet
                        ? new Views.Tablet.Dialogs.ConfirmDialog(confirmDialogParameters, CloseConfirmExitFromPaymentCallbackAsync)
                        : new Views.Mobile.Dialogs.ConfirmDialog(confirmDialogParameters, CloseConfirmExitFromPaymentCallbackAsync);

                    await PopupNavigation.PushAsync(confirmDialog);
                }
                else
                {
                    await _navigationService.GoBackAsync();
                }
            }
        }

        private async void CloseConfirmExitFromPaymentCallbackAsync(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isExitConfirmed))
            {
                await _notificationsService.CloseAllPopupAsync();

                if (isExitConfirmed)
                {
                    await _navigationService.GoBackAsync();
                }
            }
        }

        private async void ClosePaymentCompleteCallbackAsync(IDialogParameters parameters)
        {
            await _navigationService.GoBackAsync();
        }

        #endregion
    }
}
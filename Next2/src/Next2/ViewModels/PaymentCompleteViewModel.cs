using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Customers;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class PaymentCompleteViewModel : BaseViewModel
    {
        private readonly ICustomersService _customersService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly INotificationsService _notificationsService;

        private ICommand _tapPaymentOptionCommand;

        private ICommand _tapTipItemCommand;

        private List<Guid> _listGiftCardIDsToBeUpdated;

        private List<GiftCardModelDTO> _tempGiftCards;

        private decimal _subtotalWithBonus;

        public PaymentCompleteViewModel(
            INavigationService navigationService,
            ICustomersService customersService,
            IOrderService orderService,
            IMapper mapper,
            INotificationsService notificationsService,
            PaidOrderBindableModel order)
            : base(navigationService)
        {
            _customersService = customersService;
            _orderService = orderService;
            _mapper = mapper;
            _notificationsService = notificationsService;

            Order = order;

            _subtotalWithBonus = Order.BonusType == EBonusType.None
                ? Order.Subtotal
                : Order.SubtotalWithBonus;

            if (Order.Customer is not null && Order.Customer.GiftCards.Any())
            {
                Order.GiftCardsTotalFunds = Order.Customer.GiftCardsTotalFund;
                Order.RemainingGiftCardsTotalFunds = Order.GiftCardsTotalFunds;
                Order.GiftCardsNumber = Order.Customer.GiftCards.Count;
            }

            _tapPaymentOptionCommand = new AsyncCommand<PaymentItem>(OnTapPaymentOptionCommandAsync, allowsMultipleExecutions: false);

            _tapTipItemCommand = new AsyncCommand<TipItem>(OnTapTipsItemCommandAsync, allowsMultipleExecutions: false);

            Task.Run(InitPaymentOptionsAsync);

            Task.Run(InitTipsItemsAsync);
        }

        #region -- Public properties --

        public ObservableCollection<PaymentItem> PaymentOptionsItems { get; set; } = new();

        public ObservableCollection<TipItem> TipValueItems { get; set; } = new();

        public PaymentItem SelectedPaymentOption { get; set; }

        public TipItem SelectedTipItem { get; set; }

        public bool IsCleared { get; set; } = true;

        public bool IsClearedTip { get; set; } = true;

        public bool NeedSignatureReceipt { get; set; }

        public byte[] BitmapSignature { get; set; }

        public string InputValue { get; set; }

        public string InputTip { get; set; }

        public string InputGiftCardFounds { get; set; }

        public ECardPaymentStatus CardPaymentStatus { get; set; }

        public PaidOrderBindableModel Order { get; set; }

        public bool IsExpandedSummary { get; set; } = true;

        public bool IsInsufficientGiftCardFunds { get; set; }

        public bool IsPaymentComplete { get; set; } = false;

        private ICommand _tapExpandCommand;
        public ICommand TapExpandCommand => _tapExpandCommand = new Command(() => IsExpandedSummary = !IsExpandedSummary);

        private ICommand _changeCardPaymentStatusCommand;
        public ICommand ChangeCardPaymentStatusCommand => _changeCardPaymentStatusCommand ??= new AsyncCommand(OnChangeCardPaymentStatusCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearDrawPanelCommand;
        public ICommand ClearDrawPanelCommand => _clearDrawPanelCommand ??= new Command(() => IsCleared = true);

        private ICommand _tapCheckBoxSignatureReceiptCommand;
        public ICommand TapCheckBoxSignatureReceiptCommand => _tapCheckBoxSignatureReceiptCommand ??= new Command(() => NeedSignatureReceipt = !NeedSignatureReceipt);

        private ICommand _addGiftCardCommand;
        public ICommand AddGiftCardCommand => _addGiftCardCommand = new AsyncCommand(OnAddGiftCardCommandAsync, allowsMultipleExecutions: false);

        private ICommand _finishPaymentCommand;
        public ICommand FinishPaymentCommand => _finishPaymentCommand ??= new AsyncCommand(OnFinishPaymentCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(InputValue))
            {
                if (decimal.TryParse(InputValue, out decimal sum))
                {
                    Order.Cash = sum / 100;
                }
                else
                {
                    Order.Cash = 0;
                }
            }

            if (args.PropertyName == nameof(InputGiftCardFounds))
            {
                IsInsufficientGiftCardFunds = false;

                if (Order.Customer is not null && Order.Customer.GiftCards.Any())
                {
                    Order.Total += Order.GiftCard;
                    Order.GiftCard = 0;
                    Order.RemainingGiftCardsTotalFunds = Order.Customer.GiftCardsTotalFund;

                    if (decimal.TryParse(InputGiftCardFounds, out decimal sum))
                    {
                        sum /= 100;

                        if (Order.GiftCardsTotalFunds >= sum)
                        {
                            if (Order.Total >= sum)
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
                        else
                        {
                            IsInsufficientGiftCardFunds = true;
                        }
                    }
                }
                else
                {
                    if (decimal.TryParse(InputGiftCardFounds, out decimal sum))
                    {
                        IsInsufficientGiftCardFunds = sum > 0;
                    }
                }
            }

            if (args.PropertyName == nameof(InputTip))
            {
                if (decimal.TryParse(InputTip, out decimal tip))
                {
                    if (SelectedTipItem?.TipType != ETipType.Other)
                    {
                        SelectedTipItem = TipValueItems.FirstOrDefault(x => x.TipType == ETipType.Other);
                    }

                    Order.Tip = tip / 100;
                    SelectedTipItem.Text = LocalizationResourceManager.Current["CurrencySign"] + $" {Order.Tip:F2}";
                }
                else
                {
                    Order.Tip = 0;
                }

                RecalculateTotal();
            }
        }

        #endregion

        #region -- Public helpers --

        public void RecalculateTotal()
        {
            Order.PriceTax = _subtotalWithBonus * Order.TaxCoefficient;
            Order.Total = _subtotalWithBonus + Order.Tip + Order.PriceTax;
            Order.Total = Order.Total - Order.Cash - Order.GiftCard;

            var cash = Order.Cash + Order.Change;

            Order.Cash = 0;
            Order.Cash = cash;
        }

        #endregion

        #region -- Private helpers --

        private Task InitPaymentOptionsAsync()
        {
            PaymentOptionsItems = new()
            {
                new()
                {
                    PaymentType = EPaymentItems.Tips,
                    Text = LocalizationResourceManager.Current["Tips"],
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PaymentType = EPaymentItems.GiftCards,
                    Text = LocalizationResourceManager.Current["GiftCards"],
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PaymentType = EPaymentItems.Cash,
                    Text = LocalizationResourceManager.Current["Cash"],
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PaymentType = EPaymentItems.Card,
                    Text = LocalizationResourceManager.Current["Card"],
                    TapCommand = _tapPaymentOptionCommand,
                },
            };

            SelectedPaymentOption = PaymentOptionsItems[0];

            return Task.CompletedTask;
        }

        private Task InitTipsItemsAsync()
        {
            TipValueItems = new()
            {
                new()
                {
                    TipType = ETipType.NoTip,
                    Text = LocalizationResourceManager.Current["NoTip"],
                    PercentTip = 0m,
                },
                new()
                {
                    TipType = ETipType.Percent,
                    PercentTip = 0.1m,
                },
                new()
                {
                    TipType = ETipType.Percent,
                    PercentTip = 0.15m,
                },
                new()
                {
                    TipType = ETipType.Percent,
                    PercentTip = 0.2m,
                },
                new()
                {
                    TipType = ETipType.Other,
                    Text = LocalizationResourceManager.Current["Other"],
                    Value = 0m,
                },
            };

            SelectedTipItem = TipValueItems.FirstOrDefault();
            var sign = LocalizationResourceManager.Current["CurrencySign"];

            foreach (var tip in TipValueItems)
            {
                if (tip.TipType == ETipType.Percent)
                {
                    var percent = 100 * tip.PercentTip;
                    tip.Value = tip.PercentTip * _subtotalWithBonus;
                    tip.Text = $"{percent}% ({sign} {tip.Value:F2})";
                }

                tip.TapCommand = _tapTipItemCommand;
            }

            return Task.CompletedTask;
        }

        private Task OnTapTipsItemCommandAsync(TipItem? item)
        {
            if (item is not null && item.TipType != ETipType.Other)
            {
                IsClearedTip = true;
                Order.Tip = item.Value;
                IsClearedTip = false;
            }
            else
            {
                Order.Tip = 0;
            }

            RecalculateTotal();

            return Task.CompletedTask;
        }

        private async Task OnTapPaymentOptionCommandAsync(PaymentItem item)
        {
            string path = string.Empty;
            NavigationParameters navigationParams = new();

            switch (item.PaymentType)
            {
                case EPaymentItems.Cash:
                    if (!App.IsTablet)
                    {
                        Order.Cash = 0;

                        path = nameof(InputCashPage);
                        navigationParams = new NavigationParameters()
                        {
                            { Constants.Navigations.TOTAL_SUM, Order.Total },
                        };
                    }

                    break;
                case EPaymentItems.Card:
                    path = nameof(WaitingSwipeCardPage);
                    IsCleared = true;
                    break;
                case EPaymentItems.Tips:
                    if (!App.IsTablet)
                    {
                        path = nameof(TipsPage);

                        navigationParams = new NavigationParameters()
                        {
                            { Constants.Navigations.TIP_ITEMS, TipValueItems },
                        };

                        if (SelectedTipItem != null)
                        {
                            navigationParams.Add(Constants.Navigations.TIP_TYPE, SelectedTipItem.TipType);
                            navigationParams.Add(Constants.Navigations.TIP_VALUE, SelectedTipItem.Value);
                        }
                    }

                    break;
                case EPaymentItems.GiftCards:
                    if (!App.IsTablet)
                    {
                        if (Order.GiftCard > 0)
                        {
                            Order.Total += Order.GiftCard;
                            Order.GiftCard = 0;
                        }

                        path = nameof(InputGiftCardPage);
                    }

                    IsCleared = true;
                    break;
            }

            if (!App.IsTablet)
            {
                await _navigationService.NavigateAsync(path, navigationParams);
            }
        }

        private async Task OnChangeCardPaymentStatusCommandAsync()
        {
            if (NeedSignatureReceipt)
            {
                PopupPage confirmDialog = new Views.Tablet.Dialogs.PaymentCompleteDialog(ClosePaymentCompleteCallbackAsync);

                await PopupNavigation.PushAsync(confirmDialog);
            }
            else
            {
                CardPaymentStatus = ECardPaymentStatus.WaitingSignature;
            }
        }

        private async void ClosePaymentCompleteCallbackAsync(IDialogParameters parameters)
        {
            await _navigationService.GoBackAsync();
        }

        private async Task OnFinishPaymentCommandAsync()
        {
            if (IsInternetConnected)
            {
                var param = new DialogParameters
                {
                    { Constants.DialogParameterKeys.PAID_ORDER_BINDABLE_MODEL, Order },
                };

                if (SelectedTipItem != null)
                {
                    param.Add(Constants.DialogParameterKeys.TIP_VALUE_DIALOG, $"+ {SelectedTipItem.Text}");
                }

                Action<IDialogParameters> callback = async (IDialogParameters par) =>
                {
                    var isServerResponseCorrect = true;

                    if (Order.Customer is not null && Order.GiftCard != 0)
                    {
                        isServerResponseCorrect = await GiftCardFinishPayment();
                    }

                    if (isServerResponseCorrect)
                    {
                        var isOrderClosed = await CloseOrderAsync();

                        if (isOrderClosed)
                        {
                            var isReceiptSuccessfullySent = await SendReceiptAsync(par);

                            NavigationParameters navigationParameters = new();

                            if (isReceiptSuccessfullySent)
                            {
                                navigationParameters.Add(Constants.Navigations.PAYMENT_COMPLETE, string.Empty);
                            }

                            await _navigationService.ClearPopupStackAsync();
                            await _navigationService.NavigateAsync(nameof(MenuPage), navigationParameters);
                        }
                        else
                        {
                            var resultOfUpdatingGiftCards = await _customersService.UpdateAllGiftCardsToPreviousStageAsync(_tempGiftCards, _listGiftCardIDsToBeUpdated);

                            Order.Customer.GiftCards = _tempGiftCards;

                            if (!resultOfUpdatingGiftCards.IsSuccess)
                            {
                                await _notificationsService.CloseAllPopupAsync();
                                await _notificationsService.ResponseToBadRequestAsync(resultOfUpdatingGiftCards.Exception?.Message);
                            }
                        }
                    }
                };

                PopupPage finishPaymentDialog = App.IsTablet
                    ? new Views.Tablet.Dialogs.FinishPaymentDialog(param, callback)
                    : new Views.Mobile.Dialogs.FinishPaymentDialog(param, callback);

                await PopupNavigation.PushAsync(finishPaymentDialog);
            }
            else
            {
                await _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        private Task<bool> SendReceiptAsync(IDialogParameters par)
        {
            bool isReceiptPrint = false;

            if (par.TryGetValue(Constants.DialogParameterKeys.PAYMENT_COMPLETE, out EPaymentReceiptOptions options))
            {
                switch (options)
                {
                    case EPaymentReceiptOptions.PrintReceipt:
                        {
                            isReceiptPrint = true;
                            break;
                        }

                    case EPaymentReceiptOptions.SendByEmail:
                    case EPaymentReceiptOptions.SendBySMS:
                    case EPaymentReceiptOptions.WithoutReceipt:

                    default:
                        break;
                }
            }

            return Task.FromResult(isReceiptPrint);
        }

        private async Task<bool> CloseOrderAsync()
        {
            var tempCurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);

            var order = _orderService.CurrentOrder;
            order.IsCashPayment = Order.Cash > 0;
            order.OrderStatus = EOrderStatus.Closed;
            order.Close = DateTime.Now;

            var updateResult = await _orderService.UpdateCurrentOrderAsync();

            if (updateResult.IsSuccess)
            {
                await _orderService.SetEmptyCurrentOrderAsync();
            }
            else
            {
                await _notificationsService.CloseAllPopupAsync();

                _orderService.CurrentOrder = tempCurrentOrder;

                await _notificationsService.ResponseToBadRequestAsync(updateResult.Exception?.Message);
            }

            return updateResult.IsSuccess;
        }

        private async Task OnAddGiftCardCommandAsync()
        {
            if (IsInternetConnected)
            {
                if (Order.Customer is not null)
                {
                    IsInsufficientGiftCardFunds = false;

                    PopupPage giftCardDialog = new Views.Mobile.Dialogs.AddGiftCardDialog(Order.Customer.GiftCardsId, _customersService, GiftCardViewDialogCallBack);

                    await PopupNavigation.PushAsync(giftCardDialog);
                }
                else
                {
                    await _notificationsService.ShowInfoDialogAsync(
                        LocalizationResourceManager.Current["Error"],
                        LocalizationResourceManager.Current["YouAreNotMember"],
                        LocalizationResourceManager.Current["Ok"]);
                }
            }
            else
            {
                await _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        private async void GiftCardViewDialogCallBack(IDialogParameters parameters)
        {
            await _notificationsService.CloseAllPopupAsync();

            if (parameters.ContainsKey(Constants.DialogParameterKeys.GIFT_CARD_ADDED)
                && parameters.TryGetValue(Constants.DialogParameterKeys.GIFT_GARD, out GiftCardModelDTO giftCard))
            {
                var customerModel = _mapper.Map<CustomerModelDTO>(_orderService.CurrentOrder.Customer);

                var resultOfAddingGiftCard = await _customersService.AddGiftCardToCustomerAsync(customerModel, giftCard);

                if (resultOfAddingGiftCard.IsSuccess)
                {
                    var customer = Order.Customer;

                    if (customer is not null)
                    {
                        customer.GiftCards.Add(giftCard);

                        Order.RemainingGiftCardsTotalFunds = Order.GiftCardsTotalFunds = customer.GiftCardsTotalFund;
                        Order.GiftCardsNumber = Order.Customer.GiftCards.Count;

                        if (decimal.TryParse(InputGiftCardFounds, out decimal sum))
                        {
                            sum /= 100;

                            if (Order.GiftCardsTotalFunds >= sum)
                            {
                                if (Order.Total >= sum)
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
                            else
                            {
                                IsInsufficientGiftCardFunds = true;
                            }
                        }
                    }
                }
                else
                {
                    await _notificationsService.ResponseToBadRequestAsync(resultOfAddingGiftCard.Exception?.Message);
                }
            }
        }

        private async Task<bool> GiftCardFinishPayment()
        {
            var isGiftCardPaymentSuccessful = true;

            if (Order.Customer is not null && Order.Customer.GiftCards.Any())
            {
                _tempGiftCards = new();

                var giftCards = Order.Customer.GiftCards;

                foreach (var card in giftCards)
                {
                    _tempGiftCards.Add(_mapper.Map<GiftCardModelDTO>(card));
                }

                _listGiftCardIDsToBeUpdated = _customersService.RecalculateCustomerGiftCardFounds(giftCards, Order.GiftCard);

                var resultOfUpdatingGiftCards = await _customersService.UpdateAllGiftCardsAsync(giftCards, _listGiftCardIDsToBeUpdated, _tempGiftCards);

                if (!resultOfUpdatingGiftCards.IsSuccess)
                {
                    giftCards = _tempGiftCards;

                    await _notificationsService.CloseAllPopupAsync();
                    await _notificationsService.ResponseToBadRequestAsync(resultOfUpdatingGiftCards.Exception?.Message);

                    isGiftCardPaymentSuccessful = false;
                }
            }

            return isGiftCardPaymentSuccessful;
        }

        #endregion
    }
}
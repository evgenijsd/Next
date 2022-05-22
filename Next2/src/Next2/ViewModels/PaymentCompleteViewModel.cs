using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Models.Api.DTO;
using Next2.Models.API.DTO;
using Next2.Services.CustomersService;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
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
        private readonly IPopupNavigation _popupNavigation;
        private readonly ICustomersService _customersService;
        private readonly IOrderService _orderService;

        private ICommand _tapPaymentOptionCommand;

        private ICommand _tapTipItemCommand;

        private float _subtotalWithBonus;

        public PaymentCompleteViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            ICustomersService customersService,
            IOrderService orderService,
            PaidOrderBindableModel order)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;
            _customersService = customersService;
            _orderService = orderService;

            Order = order;

            _subtotalWithBonus = Order.BonusType == EBonusType.None
                ? Order.Subtotal
                : Order.SubtotalWithBonus;

            if (Order.Customer is not null && Order.Customer.GiftCards.Any())
            {
                Order.GiftCardsTotalFunds = Order.Customer.GiftCardsTotalFund;
                Order.RemainingGiftCardsTotalFunds = Order.GiftCardsTotalFunds;
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
        public ICommand FinishPaymentCommand => _finishPaymentCommand = new AsyncCommand(OnFinishPaymentCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if(args.PropertyName == nameof(InputValue))
            {
                if (float.TryParse(InputValue, out float sum))
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

                    if (float.TryParse(InputGiftCardFounds, out float sum))
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
                    if (float.TryParse(InputGiftCardFounds, out float sum))
                    {
                        IsInsufficientGiftCardFunds = sum > 0;
                    }
                }
            }

            if (args.PropertyName == nameof(InputTip))
            {
                if (float.TryParse(InputTip, out float tip))
                {
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
            Order.PriceTax = _subtotalWithBonus * Order.Tax.Value;
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

            SelectedPaymentOption = PaymentOptionsItems[3];

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
                    PercentTip = 0f,
                },
                new()
                {
                    TipType = ETipType.Percent,
                    PercentTip = 0.1f,
                },
                new()
                {
                    TipType = ETipType.Percent,
                    PercentTip = 0.15f,
                },
                new()
                {
                    TipType = ETipType.Percent,
                    PercentTip = 0.2f,
                },
                new()
                {
                    TipType = ETipType.Other,
                    Text = LocalizationResourceManager.Current["Other"],
                    Value = 0f,
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
            }
            else
            {
                IsClearedTip = false;
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

                await _popupNavigation.PushAsync(confirmDialog);
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

        private async Task OnAddGiftCardCommandAsync()
        {
            IsInsufficientGiftCardFunds = false;

            PopupPage popupPage = new Views.Mobile.Dialogs.AddGiftCardDialog(_orderService, _customersService, GiftCardViewDialogCallBack);

            await _popupNavigation.PushAsync(popupPage);
        }

        private async void GiftCardViewDialogCallBack(IDialogParameters parameters)
        {
            await _popupNavigation.PopAsync();

            if (parameters.ContainsKey(Constants.DialogParameterKeys.GIFT_CARD_ADDED))
            {
                var updatedCustomer = _orderService.CurrentOrder.Customer;

                if (updatedCustomer is not null)
                {
                    Order.Customer = new CustomerBindableModel(updatedCustomer);

                    if (Order.Customer.GiftCards.Any())
                    {
                        Order.GiftCardsTotalFunds = Order.Customer.GiftCardsTotalFund;
                        Order.RemainingGiftCardsTotalFunds = Order.GiftCardsTotalFunds;

                        if (float.TryParse(InputGiftCardFounds, out float sum))
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
            }
        }

        private void RecalculateCustomerGiftCardFounds(CustomerBindableModel customer)
        {
            double totalPrice = Order.GiftCard;

            foreach (var giftCard in customer.GiftCards)
            {
                if (totalPrice != 0)
                {
                    if (giftCard.TotalBalance > totalPrice)
                    {
                        giftCard.TotalBalance -= totalPrice;
                        totalPrice = 0;
                    }
                    else if (giftCard.TotalBalance < totalPrice)
                    {
                        totalPrice -= giftCard.TotalBalance;
                        giftCard.TotalBalance = 0;
                    }
                    else if (giftCard.TotalBalance == totalPrice)
                    {
                        giftCard.TotalBalance = 0;
                        totalPrice = 0;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private async Task OnFinishPaymentCommandAsync()
        {
            if (Order.Customer is not null && Order.Customer.GiftCards.Any())
            {
                RecalculateCustomerGiftCardFounds(Order.Customer);

                if (!Order.Customer.IsNotRegistratedCustomer)
                {
                    await _customersService.UpdateCustomerAsync(Order.Customer);
                }
                else
                {
                    foreach (var giftCardModel in Order.Customer.GiftCards)
                    {
                        if (giftCardModel.TotalBalance > 0)
                        {
                            await UpdateGiftCardAsync(giftCardModel);
                        }
                        else
                        {
                            await ActivateGiftCardAsync(giftCardModel);
                        }
                    }
                }
            }
        }

        private Task ActivateGiftCardAsync(GiftCardModelDTO giftCardModel)
        {
            return _customersService.ActivateGiftCardAsync(giftCardModel);
        }

        private Task UpdateGiftCardAsync(GiftCardModelDTO giftCardModel)
        {
            return _customersService.UpdateGiftCardAsync(giftCardModel);
        }

        #endregion
    }
}
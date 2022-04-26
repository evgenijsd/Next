using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private ICommand _tapPaymentOptionCommand;

        private ICommand _tapTipsValuesCommand;

        private float _subtotalWithBonus;

        public PaymentCompleteViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            PaidOrderBindableModel order)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;

            Order = order;

            _subtotalWithBonus = Order.BonusType == EBonusType.None ? Order.Subtotal : Order.SubtotalWithBonus;

            _tapPaymentOptionCommand = new AsyncCommand<PaymentItem>(OnTapPaymentOptionCommandAsync, allowsMultipleExecutions: false);

            _tapTipsValuesCommand = new AsyncCommand<TipsItem>(OnTapTipsValuesCommandAsync, allowsMultipleExecutions: false);

            Task.Run(InitPaymentOptionsAsync);

            Task.Run(InitTipsValuesAsync);
        }

        #region -- Public properties --

        public ObservableCollection<PaymentItem> PaymentOptionsItems { get; set; } = new();

        public ObservableCollection<TipsItem> TipsValuesItems { get; set; } = new();

        public PaymentItem SelectedPaymentOption { get; set; } = new();

        public TipsItem SelectedTipsValue { get; set; } = new();

        public bool IsCleared { get; set; } = true;

        public bool IsClearedTip { get; set; } = true;

        public bool NeedSignatureReceipt { get; set; }

        public byte[] BitmapSignature { get; set; }

        public string InputValue { get; set; }

        public string InputTips { get; set; }

        public ECardPaymentStatus CardPaymentStatus { get; set; }

        public PaidOrderBindableModel Order { get; set; }

        public bool IsExpandedSummary { get; set; } = true;

        private ICommand _tapExpandCommand;
        public ICommand TapExpandCommand => _tapExpandCommand = new Command(() => IsExpandedSummary = !IsExpandedSummary);

        private ICommand _changeCardPaymentStatusCommand;
        public ICommand ChangeCardPaymentStatusCommand => _changeCardPaymentStatusCommand ??= new AsyncCommand(OnChangeCardPaymentStatusCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearDrawPanelCommand;
        public ICommand ClearDrawPanelCommand => _clearDrawPanelCommand ??= new Command(() => IsCleared = true);

        private ICommand _tapCheckBoxSignatureReceiptCommand;
        public ICommand TapCheckBoxSignatureReceiptCommand => _tapCheckBoxSignatureReceiptCommand ??= new Command(() => NeedSignatureReceipt = !NeedSignatureReceipt);

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

            if (args.PropertyName == nameof(InputTips))
            {
                if (float.TryParse(InputTips, out float tip))
                {
                    Order.Tip = tip / 100;
                }
                else
                {
                    Order.Tip = 0;
                }

                RecalculateTotal();
            }
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
                    Text = "Tips",
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PaymentType = EPaymentItems.GiftCards,
                    Text = "Gift Cards",
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PaymentType = EPaymentItems.Cash,
                    Text = "Cash",
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PaymentType = EPaymentItems.Card,
                    Text = "Card",
                    TapCommand = _tapPaymentOptionCommand,
                },
            };

            SelectedPaymentOption = PaymentOptionsItems[3];

            return Task.CompletedTask;
        }

        private void RecalculateTotal()
        {
            float subtotalWithBonus = _subtotalWithBonus;
            Order.PriceTax = (Order.Tip + subtotalWithBonus) * Order.Tax.Value;
            Order.Total = subtotalWithBonus + Order.Tip + Order.PriceTax;
        }

        private Task InitTipsValuesAsync()
        {
            TipsValuesItems = new()
            {
                new()
                {
                    Text = "No Tip",
                    PercentTips = 0f,
                },
                new()
                {
                    PercentTips = 0.1f,
                },
                new()
                {
                    PercentTips = 0.15f,
                },
                new()
                {
                    PercentTips = 0.2f,
                },
                new()
                {
                    Text = "Other",
                    PercentTips = 1f,
                },
            };

            SelectedTipsValue = TipsValuesItems[0];
            var sign = LocalizationResourceManager.Current["CurrencySign"];

            foreach (var tip in TipsValuesItems)
            {
                if (Math.Abs(tip.PercentTips) < 1 && tip.PercentTips % 1 > 0)
                {
                    var percent = 100 * tip.PercentTips;
                    float value = tip.PercentTips * _subtotalWithBonus;
                    tip.Text = $"{percent}% ({sign} {value:F2})";
                }

                tip.TapCommand = _tapTipsValuesCommand;
            }

            return Task.CompletedTask;
        }

        private Task OnTapExpandCommandAsync()
        {
            IsExpandedSummary = !IsExpandedSummary;

            return Task.CompletedTask;
        }

        private Task OnTapTipsValuesCommandAsync(TipsItem item)
        {
            if (item.PercentTips < 1)
            {
                IsClearedTip = true;
                Order.Tip = item.PercentTips * _subtotalWithBonus;
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

                        /*if (Order.Cash == 0)
                        {
                            navigationParams = new NavigationParameters()
                            {
                                { Constants.Navigations.TOTAL_SUM, Order.Total },
                            };
                        }
                        else
                        {
                            Order.Total += Order.Cash;
                            Order.Cash = 0;
                            Order.Change = 0;

                            navigationParams = new NavigationParameters()
                            {
                                { Constants.Navigations.TOTAL_SUM, Order.Total },
                            };
                        }*/
                    }

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

        #endregion
    }
}
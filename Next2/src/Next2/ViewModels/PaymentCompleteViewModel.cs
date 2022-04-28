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
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class PaymentCompleteViewModel : BaseViewModel
    {
        private readonly IPopupNavigation _popupNavigation;

        private ICommand _tapPaymentOptionCommand;

        public PaymentCompleteViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            PaidOrderBindableModel order)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;

            Order = order;

            _tapPaymentOptionCommand = new AsyncCommand<PaymentItem>(OnTapPaymentOptionCommandAsync, allowsMultipleExecutions: false);

            Task.Run(InitPaymentOptionsAsync);
        }

        #region -- Public properties --

        public ObservableCollection<PaymentItem> PaymentOptionsItems { get; set; } = new();

        public PaymentItem SelectedPaymentOption { get; set; } = new();

        public bool IsCleared { get; set; } = true;

        public bool NeedSignatureReceipt { get; set; }

        public byte[] BitmapSignature { get; set; }

        public string InputValue { get; set; }

        public ECardPaymentStatus CardPaymentStatus { get; set; }

        public PaidOrderBindableModel Order { get; set; }

        public bool IsExpandedSummary { get; set; } = true;

        private ICommand _tapExpandCommand;
        public ICommand TapExpandCommand => _tapExpandCommand = new AsyncCommand(OnTapExpandCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changeCardPaymentStatusCommand;
        public ICommand ChangeCardPaymentStatusCommand => _changeCardPaymentStatusCommand ??= new AsyncCommand(OnChangeCardPaymentStatusCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearDrawPanelCommand;
        public ICommand ClearDrawPanelCommand => _clearDrawPanelCommand ??= new AsyncCommand(OnClearDrawPanelCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapCheckBoxSignatureReceiptCommand;
        public ICommand TapCheckBoxSignatureReceiptCommand => _tapCheckBoxSignatureReceiptCommand ??= new AsyncCommand(OnTapCheckBoxSignatureReceiptCommandAsync, allowsMultipleExecutions: false);

        private ICommand _finishPaymentDialogCallCommand;
        public ICommand FinishPaymentDialogCallCommand => _finishPaymentDialogCallCommand ??= new AsyncCommand(OnFinishPaymentDialogCallCommand, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if(args.PropertyName == nameof(InputValue))
            {
                Order.Total += Order.Cash;
                Order.Cash = 0;
                Order.Change = 0;

                if (float.TryParse(InputValue, out float sum))
                {
                    sum /= 100;

                    if (Order.Total > sum)
                    {
                        Order.Cash = sum;
                        Order.Total -= sum;
                    }
                    else
                    {
                        Order.Change = sum - Order.Total;
                        Order.Cash = Order.Total;
                        Order.Total = 0;
                    }
                }
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

        private Task OnTapExpandCommandAsync()
        {
            IsExpandedSummary = !IsExpandedSummary;

            return Task.CompletedTask;
        }

        private async Task OnTapPaymentOptionCommandAsync(PaymentItem item)
        {
            string path = string.Empty;
            NavigationParameters navigationParams = new();

            switch (item.PayemenType)
            {
                case EPaymentItems.Cash:
                    if (!App.IsTablet)
                    {
                        path = nameof(InputCashPage);

                        if (Order.Cash == 0)
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
                        }
                    }

                    break;
                case EPaymentItems.Card:
                    path = nameof(WaitingSwipeCardPage);
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

        private async Task OnClearDrawPanelCommandAsync()
        {
            IsCleared = true;
        }

        private Task OnTapCheckBoxSignatureReceiptCommandAsync()
        {
            NeedSignatureReceipt = !NeedSignatureReceipt;
            return Task.CompletedTask;
        }

        private async void ClosePaymentCompleteCallbackAsync(IDialogParameters parameters)
        {
            await _navigationService.GoBackAsync();
        }

        private async Task OnFinishPaymentDialogCallCommand()
        {
            var param = new DialogParameters
            {
                { Constants.DialogParameterKeys.PAID_ORDER_BINDABLE_MODEL, Order },
            };

            if (App.IsTablet)
            {
                await _popupNavigation.PushAsync(new Views.Tablet.Dialogs.FinishPaymentDialog(param, null));
            }
            else
            {
                await _popupNavigation.PushAsync(new Views.Mobile.Dialogs.FinishPaymentDialog(param, null));
            }
        }

        #endregion
    }
}
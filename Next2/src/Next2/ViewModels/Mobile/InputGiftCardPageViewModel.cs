using AutoMapper;
using Next2.Models;
using Next2.Services.CustomersService;
using Next2.Services.Order;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Mobile
{
    public class InputGiftCardPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly ICustomersService _customersService;
        private readonly IPopupNavigation _popupNavigation;

        public InputGiftCardPageViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            IOrderService orderService,
            ICustomersService customersService)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;
            _orderService = orderService;
            _customersService = customersService;

            Customer = _orderService.CurrentOrder.Customer;
            if (Customer is not null && Customer.GiftCards.Any())
            {
                var firstGiftCard = Customer.GiftCards.Where(row => row.Founds > 0).FirstOrDefault();
                GiftCardTotaFounds = firstGiftCard.Founds;
                RemainingGiftCardTotal = GiftCardTotaFounds;
                GiftCardNumber = firstGiftCard.GiftCardNumber;
            }
        }

        #region -- Public properties --

        public string InputGiftCardFounds { get; set; }

        public int GiftCardNumber { get; set; }

        public float RemainingGiftCardTotal { get; set; }

        public float GiftCardTotaFounds { get; set; }

        public bool IsInSufficientGiftCardFounds { get; set; }

        public bool IsErrorNotificationVisible { get; set; }

        public CustomerModel? Customer { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand = new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        private ICommand _addGiftCardCommand;
        public ICommand AddGiftCardCommand => _addGiftCardCommand = new AsyncCommand(OnAddGiftCardCommandAsync, allowsMultipleExecutions: false);

        private ICommand _displayErrorNotification;
        public ICommand DisplayErrorNotification => _displayErrorNotification = new AsyncCommand(OnDisplayErrorNotificationCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --
        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(InputGiftCardFounds))
            {
                if (Customer is not null && Customer.GiftCards.Any())
                {
                    RemainingGiftCardTotal = Customer.GiftCards.Where(row => row.Founds > 0).FirstOrDefault().Founds;
                    IsInSufficientGiftCardFounds = false;
                    if (float.TryParse(InputGiftCardFounds, out float sum))
                    {
                        sum /= 100;

                        if (GiftCardTotaFounds > sum)
                        {
                            RemainingGiftCardTotal = GiftCardTotaFounds - sum;
                        }
                        else
                        {
                            IsInSufficientGiftCardFounds = true;
                            RemainingGiftCardTotal = 0;
                        }
                    }
                }
                else
                {
                    RemainingGiftCardTotal = GiftCardTotaFounds;

                    IsInSufficientGiftCardFounds = false;
                    if (float.TryParse(InputGiftCardFounds, out float sum))
                    {
                        sum /= 100;

                        if (GiftCardTotaFounds > sum)
                        {
                            RemainingGiftCardTotal = GiftCardTotaFounds - sum;
                        }
                        else
                        {
                            IsInSufficientGiftCardFounds = true;
                            RemainingGiftCardTotal = 0;
                        }
                    }
                }
            }
        }

        #endregion

        #region -- Private methods --

        private async Task OnAddGiftCardCommandAsync()
        {
            var param = new DialogParameters
            {
                { Constants.DialogParameterKeys.MARGIN, new Thickness(0) },
            };
            PopupPage popupPage = new Views.Mobile.Dialogs.AddGiftCardDialog(param, TipViewDialogCallBack, _orderService, _customersService);
            await _popupNavigation.PushAsync(popupPage);
        }

        private async void TipViewDialogCallBack(IDialogParameters parameters)
        {
            await _popupNavigation.PopAsync();
            Customer = _orderService.CurrentOrder.Customer;
            if (Customer is not null && parameters.ContainsKey(Constants.DialogParameterKeys.GIFT_CARD_ADDED))
            {
                var firstGiftCard = Customer.GiftCards.Where(row => row.Founds > 0).FirstOrDefault();
                GiftCardTotaFounds = firstGiftCard.Founds;
                RemainingGiftCardTotal = GiftCardTotaFounds;
                GiftCardNumber = firstGiftCard.GiftCardNumber;
            }
        }

        private Task OnGoBackCommandAsync()
        {
            var navigationParam = new NavigationParameters();

            if (Customer is not null)
            {
                if (Customer.IsUpdatedCustomer)
                {
                    navigationParam = new NavigationParameters()
                    {
                        { Constants.Navigations.GIFT_CARD_FOUNDS, InputGiftCardFounds },
                        { Constants.Navigations.GIFT_CARD_ADDED, true },
                        { Constants.Navigations.GIFT_CARD_NUMBER, GiftCardNumber },
                    };
                }
                else
                {
                    navigationParam = new NavigationParameters()
                    {
                        { Constants.Navigations.GIFT_CARD_NUMBER, GiftCardNumber },
                        { Constants.Navigations.GIFT_CARD_FOUNDS, InputGiftCardFounds },
                    };
                }
            }

            return _navigationService.GoBackAsync(navigationParam);
        }

        private Task OnDisplayErrorNotificationCommandAsync()
        {
            IsErrorNotificationVisible = true;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await System.Threading.Tasks.Task.Delay(2000);
                IsErrorNotificationVisible = !IsErrorNotificationVisible;
            });
            return Task.CompletedTask;
        }

        #endregion
    }
}

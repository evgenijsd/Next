﻿using AutoMapper;
using Next2.Models;
using Next2.Models.API.DTO;
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
                RemainingGiftCardTotal = Customer.GiftCardsTotalFund;
            }
        }

        #region -- Public properties --

        public string InputGiftCardFounds { get; set; }

        public float RemainingGiftCardTotal { get; set; }

        public bool IsInSufficientGiftCardFunds { get; set; }

        public bool IsErrorNotificationVisible { get; set; }

        public CustomerModelDTO? Customer { get; set; }

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
                IsInSufficientGiftCardFunds = false;
                RemainingGiftCardTotal = 0;

                if (Customer is not null && Customer.GiftCards.Any())
                {
                    RemainingGiftCardTotal = Customer.GiftCardsTotalFund;

                    if (float.TryParse(InputGiftCardFounds, out float sum))
                    {
                        sum /= 100;

                        if (Customer.GiftCardsTotalFund >= sum)
                        {
                            RemainingGiftCardTotal = Customer.GiftCardsTotalFund - sum;
                        }
                        else
                        {
                            IsInSufficientGiftCardFunds = true;
                            RemainingGiftCardTotal = 0;
                        }
                    }
                }
                else
                {
                    if (float.TryParse(InputGiftCardFounds, out float sum))
                    {
                        IsInSufficientGiftCardFunds = RemainingGiftCardTotal < sum;
                    }
                }
            }
        }

        #endregion

        #region -- Private methods --

        private Task OnAddGiftCardCommandAsync()
        {
            IsInSufficientGiftCardFunds = false;

            PopupPage popupPage = new Views.Mobile.Dialogs.AddGiftCardDialog(_orderService, _customersService, AddGiftCardDialogCallback);

            return _popupNavigation.PushAsync(popupPage);
        }

        private async void AddGiftCardDialogCallback(IDialogParameters parameters)
        {
            await _popupNavigation.PopAsync();

            if (_orderService.CurrentOrder.Customer is not null)
            {
                Customer = new(_orderService.CurrentOrder.Customer);

                if (parameters.ContainsKey(Constants.DialogParameterKeys.GIFT_CARD_ADDED))
                {
                    if (float.TryParse(InputGiftCardFounds, out float sum))
                    {
                        sum /= 100;

                        RemainingGiftCardTotal = Customer.GiftCardsTotalFund - sum;
                    }
                    else
                    {
                        RemainingGiftCardTotal = Customer.GiftCardsTotalFund;
                    }
                }
            }
        }

        private Task OnGoBackCommandAsync()
        {
            var navigationParam = new NavigationParameters();

            if (Customer is not null)
            {
                navigationParam.Add(Constants.Navigations.GIFT_CARD_FOUNDS, InputGiftCardFounds);

                if (Customer.IsUpdatedCustomer)
                {
                    navigationParam.Add(Constants.Navigations.GIFT_CARD_ADDED, true);
                }
            }

            return _navigationService.GoBackAsync(navigationParam);
        }

        private Task OnDisplayErrorNotificationCommandAsync()
        {
            IsErrorNotificationVisible = true;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(2000);
                IsErrorNotificationVisible = false;
            });

            return Task.CompletedTask;
        }

        #endregion
    }
}
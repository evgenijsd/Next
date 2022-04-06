﻿using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Authentication;
using Next2.Services.Order;
using Next2.Views.Tablet.Dialogs;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Tablet
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IPopupNavigation _popupNavigation;
        private readonly IOrderService _orderService;

        public MenuPageViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            IAuthenticationService authenticationService,
            NewOrderViewModel newOrderViewModel,
            HoldItemsViewModel holdItemsViewModel,
            OrderTabsViewModel orderTabsViewModel,
            ReservationsViewModel reservationsViewModel,
            MembershipViewModel membershipViewModel,
            CustomersViewModel customersViewModel,
            SettingsViewModel settingsViewModel,
            IOrderService orderService)
            : base(navigationService)
        {
            NewOrderViewModel = newOrderViewModel;
            HoldItemsViewModel = holdItemsViewModel;
            OrderTabsViewModel = orderTabsViewModel;
            ReservationsViewModel = reservationsViewModel;
            MembershipViewModel = membershipViewModel;
            CustomersViewModel = customersViewModel;
            SettingsViewModel = settingsViewModel;
            _authenticationService = authenticationService;
            _popupNavigation = popupNavigation;
            _orderService = orderService;

            InitMenuItems();

            MessagingCenter.Subscribe<MenuPageSwitchingMessage>(this, Constants.Navigations.SWITCH_PAGE, PageSwitchingMessageHandler);
        }

        #region -- Public properties --

        private ICommand _logOutCommand;
        public ICommand LogOutCommand => _logOutCommand ??= new AsyncCommand(OnLogOutCommandAsync, allowsMultipleExecutions: false);

        private MenuItemBindableModel _selectedMenuItem;
        public MenuItemBindableModel SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                _selectedMenuItem?.ViewModel?.OnDisappearing();
                SetProperty(ref _selectedMenuItem, value);

                _selectedMenuItem?.ViewModel?.OnAppearing();
            }
        }

        public ObservableCollection<MenuItemBindableModel> MenuItems { get; set; }

        public NewOrderViewModel NewOrderViewModel { get; set; }

        public HoldItemsViewModel HoldItemsViewModel { get; set; }

        public OrderTabsViewModel OrderTabsViewModel { get; set; }

        public ReservationsViewModel ReservationsViewModel { get; set; }

        public MembershipViewModel MembershipViewModel { get; set; }

        public CustomersViewModel CustomersViewModel { get; set; }

        public SettingsViewModel SettingsViewModel { get; set; }

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters is not null && parameters.ContainsKey(Constants.Navigations.REFRESH_ORDER))
            {
                NewOrderViewModel.OrderRegistrationViewModel.RefreshCurrentOrderAsync();
            }
        }

        #endregion

        #region -- Private methods --

        private void PageSwitchingMessageHandler(MenuPageSwitchingMessage sender)
        {
            if (sender is not null)
            {
                var targetPage = MenuItems.FirstOrDefault(x => x.State == sender.Page);

                if (targetPage is not null)
                {
                    SelectedMenuItem = targetPage;
                }
            }
        }

        private void InitMenuItems()
        {
            MenuItems = new()
            {
                new MenuItemBindableModel()
                {
                    State = EMenuItems.NewOrder,
                    Title = "New Order",
                    ImagePath = "ic_plus_30x30.png",
                    ViewModel = NewOrderViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.HoldItems,
                    Title = "Hold Items",
                    ImagePath = "ic_time_circle_30x30.png",
                    ViewModel = HoldItemsViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.OrderTabs,
                    Title = "Order & Tabs",
                    ImagePath = "ic_folder_30x30.png",
                    ViewModel = OrderTabsViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Reservations,
                    Title = "Reservations",
                    ImagePath = "ic_bookmark_30x30.png",
                    ViewModel = ReservationsViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Membership,
                    Title = "Membership",
                    ImagePath = "ic_work_30x30.png",
                    ViewModel = MembershipViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Customers,
                    Title = "Customers",
                    ImagePath = "ic_user_30x30.png",
                    ViewModel = CustomersViewModel,
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Settings,
                    Title = "Settings",
                    ImagePath = "ic_setting_30x30.png",
                    ViewModel = SettingsViewModel,
                },
            };

            SelectedMenuItem = MenuItems.FirstOrDefault();
        }

        private async Task OnLogOutCommandAsync()
        {
            var dialogParameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["WantToLogOut"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["LogOut_UpperCase"] },
            };

            PopupPage confirmDialog = App.IsTablet
                ? new Next2.Views.Tablet.Dialogs.ConfirmDialog(dialogParameters, CloseDialogCallback)
                : new Next2.Views.Mobile.Dialogs.ConfirmDialog(dialogParameters, CloseDialogCallback);

            await _popupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDialogCallback(IDialogParameters dialogResult)
        {
            bool result = (bool)dialogResult?[Constants.DialogParameterKeys.ACCEPT];

            if (result)
            {
                await _orderService.CreateNewOrderAsync();
                _authenticationService.LogOut();

                await _popupNavigation.PopAsync();

                var navigationParameters = new NavigationParameters
                {
                    { Constants.Navigations.IS_LAST_USER_LOGGED_OUT, result },
                };

                await _navigationService.GoBackToRootAsync(navigationParameters);
            }
            else
            {
                await _popupNavigation.PopAsync();
            }
        }
        #endregion
    }
}

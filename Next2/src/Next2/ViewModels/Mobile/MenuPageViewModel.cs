﻿using Next2.Enums;
using Next2.Models;
using Next2.Services.Menu;
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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;

        private readonly IPopupNavigation _popupNavigation;

        private MenuItemBindableModel _oldSelectedMenuItem;

        public MenuPageViewModel(
            INavigationService navigationService,
            IMenuService menuService,
            IPopupNavigation popupNavigation,
            IOrderService orderService)
            : base(navigationService)
        {
            _menuService = menuService;
            _orderService = orderService;
            _popupNavigation = popupNavigation;

            CanShowOrder = _orderService.CurrentOrder.Seats.Count > 0;

            InitMenuItems();
            Task.Run(LoadCategoriesAsync);
        }

        #region -- Public properties --

        public bool CanShowOrder { get; set; }

        public ObservableCollection<MenuItemBindableModel> MenuItems { get; set; }

        public MenuItemBindableModel SelectedMenuItem { get; set; }

        public ObservableCollection<CategoryModel> Categories { get; set; }

        private ICommand _tapCategoryCommand;
        public ICommand TapCategoryCommand => _tapCategoryCommand ??= new AsyncCommand<CategoryModel>(OnTapCategoryCommandAsync, allowsMultipleExecutions: false);
        private ICommand _openNewOrderPageCommand;
        public ICommand OpenNewOrderPageCommand => _openNewOrderPageCommand ??= new AsyncCommand(OnOpenNewOrderPageCommandAsync, allowsMultipleExecutions: false);

        private ICommand _goToSettingsCommand;
        public ICommand GoToSettingsCommand => _goToSettingsCommand ??= new AsyncCommand(GoToSettingsCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters is not null && parameters.ContainsKey(Constants.Navigations.PAYMENT_COMPLETE))
            {
                PopupPage confirmDialog = new Views.Mobile.Dialogs.PaymentCompleteDialog((IDialogParameters par) => _popupNavigation.PopAsync());

                await PopupNavigation.PushAsync(confirmDialog);
            }

            SelectedMenuItem = MenuItems.FirstOrDefault();
            _oldSelectedMenuItem = SelectedMenuItem;

            CanShowOrder = _orderService.CurrentOrder.Seats.Count > 0;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SelectedMenuItem))
            {
                if (_oldSelectedMenuItem.State == EMenuItems.NewOrder)
                {
                    _oldSelectedMenuItem = SelectedMenuItem;

                    switch (SelectedMenuItem.State)
                    {
                        case EMenuItems.HoldItems:
                            _navigationService.NavigateAsync(nameof(HoldItemsPage));
                            break;
                        case EMenuItems.OrderTabs:
                            _navigationService.NavigateAsync(nameof(OrderTabsPage));
                            break;
                        case EMenuItems.Customers:
                            _navigationService.NavigateAsync(nameof(CustomersPage));
                            break;
                    }
                }
            }
        }

        #endregion

        #region -- Private helpers --

        private void InitMenuItems()
        {
            MenuItems = new ()
            {
                new MenuItemBindableModel()
                {
                    State = EMenuItems.NewOrder,
                    Title = "New Order",
                    ImagePath = "ic_plus_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.HoldItems,
                    Title = "Hold Items",
                    ImagePath = "ic_time_circle_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.OrderTabs,
                    Title = "Order & Tabs",
                    ImagePath = "ic_folder_30x30.png",
                },
                new MenuItemBindableModel()
                {
                    State = EMenuItems.Customers,
                    Title = "Customers",
                    ImagePath = "ic_user_30x30.png",
                },
            };

            _oldSelectedMenuItem = MenuItems.FirstOrDefault();
            SelectedMenuItem = _oldSelectedMenuItem;
        }

        private async Task LoadCategoriesAsync()
        {
            if (IsInternetConnected)
            {
                var resultCategories = await _menuService.GetAllCategoriesAsync();

                if (resultCategories.IsSuccess)
                {
                    Categories = new(resultCategories.Result);
                }
            }
        }

        private Task OnOpenNewOrderPageCommandAsync()
        {
            return CanShowOrder ? _navigationService.NavigateAsync(nameof(OrderRegistrationPage)) : Task.CompletedTask;
        }

        private async Task OnTapCategoryCommandAsync(CategoryModel category)
        {
            var navigationParams = new NavigationParameters
            {
                { Constants.Navigations.CATEGORY, category },
            };

            await _navigationService.NavigateAsync(nameof(ChooseSetPage), navigationParams);
        }

        private async Task GoToSettingsCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(SettingsPage)}");
        }

        #endregion
    }
}
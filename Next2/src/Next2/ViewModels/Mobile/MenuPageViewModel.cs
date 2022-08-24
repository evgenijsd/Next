﻿using Next2.Enums;
using Next2.Models;
using Next2.Services.Authentication;
using Next2.Services.Menu;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;

        private MenuItemBindableModel _oldSelectedMenuItem = new();

        public MenuPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            IMenuService menuService,
            IOrderService orderService)
            : base(navigationService, authenticationService, notificationsService)
        {
            _menuService = menuService;
            _orderService = orderService;

            CanShowOrder = _orderService.CurrentOrder.Seats.Count > 0;

            InitMenuItems();

            Task.Run(OnRefreshCategoriesCommandAsync);
        }

        #region -- Public properties --

        public ELoadingState CategoriesLoadingState { get; set; } = ELoadingState.InProgress;

        public bool CanShowOrder { get; set; }

        public ObservableCollection<MenuItemBindableModel> MenuItems { get; set; } = new();

        public MenuItemBindableModel SelectedMenuItem { get; set; }

        public ObservableCollection<CategoryModel> Categories { get; set; } = new();

        private ICommand? _tapCategoryCommand;
        public ICommand TapCategoryCommand => _tapCategoryCommand ??= new AsyncCommand<CategoryModel>(OnTapCategoryCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _openNewOrderPageCommand;
        public ICommand OpenNewOrderPageCommand => _openNewOrderPageCommand ??= new AsyncCommand(OnOpenNewOrderPageCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _goToSettingsCommand;
        public ICommand GoToSettingsCommand => _goToSettingsCommand ??= new AsyncCommand(GoToSettingsCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _refreshCategoriesCommand;
        public ICommand RefreshCategoriesCommand => _refreshCategoriesCommand ??= new AsyncCommand(OnRefreshCategoriesCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey(Constants.Navigations.IS_FIRST_ORDER_INIT))
            {
                await _orderService.OpenLastOrCreateNewOrderAsync();
            }

            if (parameters.ContainsKey(Constants.Navigations.PAYMENT_COMPLETE))
            {
                PopupPage confirmDialog = new Views.Mobile.Dialogs.PaymentCompleteDialog((IDialogParameters par) => PopupNavigation.PopAsync());

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
                            _navigationService.NavigateAsync(nameof(HoldDishesPage));
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

        private async Task OnRefreshCategoriesCommandAsync()
        {
            CategoriesLoadingState = ELoadingState.InProgress;

            if (IsInternetConnected)
            {
                var resultCategories = await _menuService.GetAllCategoriesAsync();

                if (resultCategories.IsSuccess)
                {
                    Categories = new(resultCategories.Result);
                    CategoriesLoadingState = ELoadingState.Completed;
                }
                else if (resultCategories.Exception?.Message == Constants.StatusCode.UNAUTHORIZED)
                {
                    await PerformLogoutAsync();
                }
                else
                {
                    CategoriesLoadingState = ELoadingState.Error;
                }
            }
            else
            {
                CategoriesLoadingState = ELoadingState.NoInternet;
            }
        }

        private async Task OnOpenNewOrderPageCommandAsync()
        {
            if (IsInternetConnected)
            {
                if (CanShowOrder)
                {
                    await _navigationService.NavigateAsync(nameof(OrderRegistrationPage));
                }
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private Task OnTapCategoryCommandAsync(CategoryModel? category)
        {
            var navigationParams = new NavigationParameters
            {
                { Constants.Navigations.CATEGORY, category },
            };

            return IsInternetConnected
                ? _navigationService.NavigateAsync(nameof(ChooseDishPage), navigationParams)
                : _notificationsService.ShowNoInternetConnectionDialogAsync();
        }

        private Task GoToSettingsCommandAsync()
        {
            return IsInternetConnected
                ? _navigationService.NavigateAsync($"{nameof(SettingsPage)}")
                : _notificationsService.ShowNoInternetConnectionDialogAsync();
        }

        #endregion
    }
}
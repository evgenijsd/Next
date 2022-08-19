using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Models.Bindables;
using Next2.Services.Authentication;
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
using Xamarin.Forms;

namespace Next2.ViewModels.Tablet
{
    public class MenuPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        public MenuPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            NewOrderViewModel newOrderViewModel,
            HoldDishesViewModel holdDishesViewModel,
            OrderTabsViewModel orderTabsViewModel,
            ReservationsViewModel reservationsViewModel,
            MembershipViewModel membershipViewModel,
            CustomersViewModel customersViewModel,
            SettingsViewModel settingsViewModel,
            IOrderService orderService)
            : base(navigationService, authenticationService, notificationsService)
        {
            NewOrderViewModel = newOrderViewModel;
            HoldDishesViewModel = holdDishesViewModel;
            OrderTabsViewModel = orderTabsViewModel;
            ReservationsViewModel = reservationsViewModel;
            MembershipViewModel = membershipViewModel;
            CustomersViewModel = customersViewModel;
            SettingsViewModel = settingsViewModel;
            _orderService = orderService;

            InitMenuItems();

            MessagingCenter.Subscribe<MenuPageSwitchingMessage>(this, Constants.Navigations.SWITCH_PAGE, PageSwitchingMessageHandler);
        }

        #region -- Public properties --

        private ICommand? _logOutCommand;
        public ICommand LogOutCommand => _logOutCommand ??= new AsyncCommand(OnLogOutCommandAsync, allowsMultipleExecutions: false);

        private MenuItemBindableModel? _selectedMenuItem;
        public MenuItemBindableModel? SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                _selectedMenuItem?.ViewModel?.OnDisappearing();

                SetProperty(ref _selectedMenuItem, value);

                _selectedMenuItem?.ViewModel?.OnAppearing();
            }
        }

        public ObservableCollection<MenuItemBindableModel> MenuItems { get; set; } = new();

        public NewOrderViewModel NewOrderViewModel { get; set; }

        public HoldDishesViewModel HoldDishesViewModel { get; set; }

        public OrderTabsViewModel OrderTabsViewModel { get; set; }

        public ReservationsViewModel ReservationsViewModel { get; set; }

        public MembershipViewModel MembershipViewModel { get; set; }

        public CustomersViewModel CustomersViewModel { get; set; }

        public SettingsViewModel SettingsViewModel { get; set; }

        #endregion

        #region -- Overrides --

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.Count > 0)
            {
                if (parameters.ContainsKey(Constants.Navigations.IS_FIRST_ORDER_INIT))
                {
                    await _orderService.OpenLastOrCreateNewOrderAsync();
                    await NewOrderViewModel.OrderRegistrationViewModel.RefreshCurrentOrderAsync();
                }

                if (parameters.ContainsKey(Constants.Navigations.PAYMENT_COMPLETE))
                {
                    PopupPage confirmDialog = new Views.Tablet.Dialogs.PaymentCompleteDialog((IDialogParameters par) => PopupNavigation.PopAsync());

                    await PopupNavigation.PushAsync(confirmDialog);
                }

                if (parameters.ContainsKey(Constants.Navigations.REFRESH_ORDER))
                {
                    await NewOrderViewModel.OrderRegistrationViewModel.RefreshCurrentOrderAsync();
                }

                if (parameters.TryGetValue(Constants.Navigations.SEARCH_QUERY, out string searchQuery))
                {
                    OrderTabsViewModel.SetSearchQuery(searchQuery);
                }

                if (parameters.TryGetValue(Constants.Navigations.SEARCH_MEMBER, out string searchMember))
                {
                    MembershipViewModel.SetSearchQuery(searchMember);
                }

                if (parameters.TryGetValue(Constants.Navigations.SEARCH_CUSTOMER, out string searchCustomer))
                {
                    CustomersViewModel.SetSearchQuery(searchCustomer);
                }

                if (parameters.TryGetValue(Constants.Navigations.SEARCH_RESERVATION, out string searchReservation))
                {
                    ReservationsViewModel.SetSearchQuery(searchReservation);
                }

                if (parameters.ContainsKey(Constants.Navigations.TAX_OFF))
                {
                    await NewOrderViewModel.OrderRegistrationViewModel.RemoveTaxAsync();
                }

                if (parameters.TryGetValue(Constants.Navigations.BONUS, out FullOrderBindableModel currentOrder))
                {
                    await NewOrderViewModel.OrderRegistrationViewModel.UpdateOrderWithBonusAsync(currentOrder);
                }
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SelectedMenuItem))
            {
                NewOrderViewModel.OrderRegistrationViewModel.IsClockRunning = SelectedMenuItem?.State == EMenuItems.NewOrder;
            }
        }

        #endregion

        #region -- Private helpers --

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

            MessagingCenter.Unsubscribe<MenuPageSwitchingMessage>(sender, Constants.Navigations.SWITCH_PAGE);
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
                    ViewModel = HoldDishesViewModel,
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

        private Task OnLogOutCommandAsync()
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

            return PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseDialogCallback(IDialogParameters dialogResult)
        {
            if (dialogResult.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool accept))
            {
                if (accept)
                {
                    await _notificationsService.CloseAllPopupAsync();

                    var logoutResult = await _authenticationService.LogoutAsync();

                    if (logoutResult.IsSuccess)
                    {
                        NewOrderViewModel.OrderRegistrationViewModel.CurrentState = ENewOrderViewState.InProgress;

                        _orderService.CurrentOrder = new();

                        var navigationParameters = new NavigationParameters
                        {
                            { Constants.Navigations.LOGOUT, true },
                        };

                        await _navigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(LoginPage)}");
                    }
                }
                else
                {
                    await _notificationsService.CloseAllPopupAsync();
                }
            }
        }

        #endregion
    }
}

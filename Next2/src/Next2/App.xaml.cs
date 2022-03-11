using Next2.Resources.Strings;
using Next2.Services.Authentication;
using Next2.Services.CustomersService;
using Next2.Services.Membership;
using Next2.Services.Menu;
using Next2.Services.Mock;
using Next2.Services.Order;
using Next2.Services.SettingsService;
using Next2.Services.UserService;
using Next2.ViewModels;
using Next2.ViewModels.Dialogs;
using Next2.ViewModels.Tablet;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Prism.Unity;
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MobileViewModels = Next2.ViewModels.Mobile;
using MobileViews = Next2.Views.Mobile;
using TabletViewModels = Next2.ViewModels.Tablet;
using TabletViews = Next2.Views.Tablet;
using AutoMapper;
using Next2.Models;
using Next2.ViewModels.Mobile;

namespace Next2
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer? initializer = null)
            : base(initializer)
        {
        }

        #region -- Public properties --

        public static bool IsTablet = Xamarin.Forms.Device.Idiom == TargetIdiom.Tablet;

        #endregion

        #region -- Overrides --

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Dialogs
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterPopupDialogService();

            //Services
            var mapper = CreateMapper();
            containerRegistry.RegisterInstance(mapper);
            containerRegistry.RegisterSingleton<IMockService, MockService>();
            containerRegistry.RegisterSingleton<IOrderService, OrderService>();
            containerRegistry.RegisterSingleton<IMenuService, MenuService>();
            containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
            containerRegistry.RegisterSingleton<IUserService, UserService>();
            containerRegistry.RegisterSingleton<IAuthenticationService, AuthenticationService>();
            containerRegistry.RegisterSingleton<ICustomersService, CustomersService>();

            // Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();

            if (IsTablet)
            {
                // Services
                containerRegistry.RegisterSingleton<IMembershipService, MembershipService>();

                //Navigation
                containerRegistry.RegisterForNavigation<TabletViews.SearchPage, SearchPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.MenuPage, TabletViewModels.MenuPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.ExpandPage, TabletViewModels.ExpandPageViewModel>();

                containerRegistry.RegisterSingleton<NewOrderViewModel>();
                containerRegistry.RegisterSingleton<HoldItemsViewModel>();
                containerRegistry.RegisterSingleton<OrderTabsViewModel>();
                containerRegistry.RegisterSingleton<ReservationsViewModel>();
                containerRegistry.RegisterSingleton<CustomersViewModel>();
                containerRegistry.RegisterSingleton<MembershipViewModel>();
                containerRegistry.RegisterSingleton<SettingsViewModel>();
                containerRegistry.RegisterSingleton<OrderRegistrationViewModel>();
                containerRegistry.RegisterDialog<TabletViews.Dialogs.LogOutAlertView, LogOutAlertViewModel>();
                containerRegistry.RegisterDialog<TabletViews.Dialogs.CustomerInfoDialog, CustomerInfoViewModel>();
                containerRegistry.RegisterDialog<TabletViews.Dialogs.CustomerAddDialog, CustomerInfoViewModel>();
            }
            else
            {
                containerRegistry.RegisterForNavigation<MobileViews.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.EditPage, EditPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.LoginPage_EmployeeId, LoginPage_EmployeeIdViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.SettingsPage, SettingsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.MenuPage, MobileViewModels.MenuPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.HoldItemsPage, HoldItemsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.OrderRegistrationPage, OrderRegistrationViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.OrderTabsPage, OrderTabsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.CustomersPage, CustomersViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.ChooseSetPage, MobileViewModels.ChooseSetPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.SearchPage, SearchPageViewModel>();
                containerRegistry.RegisterDialog<MobileViews.Dialogs.CustomerAddDialog, CustomerInfoViewModel>();
                containerRegistry.RegisterDialog<MobileViews.Dialogs.CustomerInfoDialog, CustomerInfoViewModel>();
            }
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            App.Current.UserAppTheme = OSAppTheme.Dark;

#if !DEBUG
            AppCenter.Start(
                $"ios={Constants.Analytics.IOSKey};android={Constants.Analytics.AndroidKey};",
                typeof(Analytics),
                typeof(Crashes));

            await Analytics.SetEnabledAsync(true);
#endif
            InitializeComponent();

            LocalizationResourceManager.Current.Init(Strings.ResourceManager);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(MobileViews.LoginPage)}");
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        #endregion

        #region --- Private helpers --

        private IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TableModel, TableBindableModel>();
                cfg.CreateMap<CustomerModel, CustomerBindableModel>();
                cfg.CreateMap<MemberModel, MemberBindableModel>();
            }).CreateMapper();
        }

        #endregion
    }
}
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Next2.Resources.Strings;
using Next2.Services.Authentication;
using Next2.Services.Membership;
using Next2.Services.Menu;
using Next2.Services.MockService;
using Next2.Services.ProfileService;
using Next2.Services.OrderService;
using Next2.ViewModels;
using Next2.ViewModels.Tablet;
using Next2.Views.Tablet;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Prism.Unity;
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using MobileViewModels = Next2.ViewModels.Mobile;
using MobileViews = Next2.Views.Mobile;
using TabletViewModels = Next2.ViewModels.Tablet;
using TabletViews = Next2.Views.Tablet;
using Next2.Services.SettingsService;

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
            containerRegistry.RegisterSingleton<IMockService, MockService>();
            containerRegistry.RegisterSingleton<IOrderService, OrderService>();
            containerRegistry.RegisterSingleton<IMembershipService, MembershipService>();
            containerRegistry.RegisterSingleton<IMenuService, MenuService>();
            containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
            containerRegistry.RegisterSingleton<IUserService, UserService>();
            containerRegistry.RegisterSingleton<IAuthenticationService, AuthenticationService>();

            // Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            if (IsTablet)
            {
                // Services
                containerRegistry.RegisterSingleton<IMembershipService, MembershipService>();

                //Navigation
                containerRegistry.RegisterForNavigation<TabletViews.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterSingleton<IMembershipService, MembershipService>();
                containerRegistry.RegisterForNavigation<TabletViews.MenuPage, TabletViewModels.MenuPageViewModel>();
                containerRegistry.RegisterSingleton<NewOrderViewModel>();
                containerRegistry.RegisterSingleton<HoldItemsViewModel>();
                containerRegistry.RegisterSingleton<OrderTabsViewModel>();
                containerRegistry.RegisterSingleton<ReservationsViewModel>();
                containerRegistry.RegisterSingleton<MembershipViewModel>();
                containerRegistry.RegisterSingleton<CustomersViewModel>();
                containerRegistry.RegisterSingleton<SettingsViewModel>();
                containerRegistry.RegisterDialog<TabletViews.Dialogs.LogOutAlertView, LogOutAlertViewModel>();
            }
            else
            {
                containerRegistry.RegisterForNavigation<MobileViews.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.LoginPage_EmployeeId, LoginPage_EmployeeIdViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.SettingsPage, SettingsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.MenuPage, MobileViewModels.MenuPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.HoldItemsPage, HoldItemsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.OrderTabsPage, OrderTabsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.CustomersPage, CustomersViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.ChooseSetPage, MobileViewModels.ChooseSetPageViewModel>();
            }
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            App.Current.UserAppTheme = OSAppTheme.Dark;
#if !DEBUG
            AppCenter.Start(
                $"ios={Constants.Analytics.IOSKey};android={Constants.Analytics.AndroidKey};",
                typeof(Analytics),
                typeof(Crashes));

            await Analytics.SetEnabledAsync(true);
#endif
            InitializeComponent();

            LocalizationResourceManager.Current.Init(Strings.ResourceManager);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(LoginPage)}");
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
    }
}
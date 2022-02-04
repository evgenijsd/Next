using Next2.Services;
using Next2.Services.Authentication;
using Next2.Services.ProfileService;
using Next2.Services.Services;
using Next2.ViewModels;
using Next2.ViewModels.Mobile;
using Next2.Views;
using Next2.Views.Tablet;
using Next2.Resources.Strings;
using Next2.Services;
using Next2.Services.Membership;
using Next2.ViewModels;
using MobileViews = Next2.Views.Mobile;
using TabletViews = Next2.Views.Tablet;
using MobileViewModels = Next2.ViewModels.Mobile;
using TabletViewModels = Next2.ViewModels.Tablet;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Next2.ViewModels.Tablet;

namespace Next2
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null)
            : base(initializer)
        {
        }

        #region -- Public properties --

        public static bool IsTablet = Xamarin.Forms.Device.Idiom == TargetIdiom.Tablet;

        #endregion

        #region -- Overrides --

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Services
            containerRegistry.RegisterSingleton<IMockService, MockService>();
            containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
            containerRegistry.RegisterSingleton<IUserService, UserService>();
            containerRegistry.RegisterSingleton<IAuthenticationService, AuthenticationService>();

            // Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            if (Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            {
                containerRegistry.RegisterForNavigation<Views.Mobile.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterForNavigation<Views.Mobile.LoginPage_EmployeeId, LoginPage_EmployeeIdViewModel>();
                containerRegistry.RegisterForNavigation<Views.StartPage, StartPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.MenuPage, MobileViewModels.MenuPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.HoldItemsPage, HoldItemsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.OrderTabsPage, OrderTabsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.CustomersPage, CustomersViewModel>();
            }
            else
            {
                containerRegistry.RegisterForNavigation<Views.Tablet.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterForNavigation<Views.StartPage, StartPageViewModel>();
                containerRegistry.RegisterSingleton<IMembershipService, MembershipService>();
                containerRegistry.RegisterForNavigation<TabletViews.MenuPage, TabletViewModels.MenuPageViewModel>();

                containerRegistry.RegisterSingleton<NewOrderViewModel>();
                containerRegistry.RegisterSingleton<HoldItemsViewModel>();
                containerRegistry.RegisterSingleton<OrderTabsViewModel>();
                containerRegistry.RegisterSingleton<ReservationsViewModel>();
                containerRegistry.RegisterSingleton<MembershipViewModel>();
                containerRegistry.RegisterSingleton<CustomersViewModel>();
                containerRegistry.RegisterSingleton<SettingsViewModel>();
            }
        }

        protected override async void OnInitialized()
        {
#if !DEBUG
            AppCenter.Start(
                $"ios={Constants.Analytics.IOSKey};android={Constants.Analytics.AndroidKey};",
                typeof(Analytics),
                typeof(Crashes));

            await Analytics.SetEnabledAsync(true);
#endif
            InitializeComponent();

            await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(LoginPage)}");

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
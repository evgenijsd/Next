using Next2.Services;
using Next2.ViewModels;
using Next2.Views;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Xamarin.Forms;

namespace Next2
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer? initializer = null)
            : base(initializer)
        {
        }

        #region -- Overrides --

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Services
            containerRegistry.RegisterSingleton<IMockService, MockService>();
            containerRegistry.RegisterSingleton<IOrderService, OrderService>();

            // Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<StartPage, StartPageViewModel>();
            containerRegistry.RegisterForNavigation<Views.OrderPageMobile, ViewModels.OrderPageMobileViewModel>();
            containerRegistry.RegisterForNavigation<Views.TabPageMobile, ViewModels.TabPageMobileViewModel>();
            containerRegistry.RegisterForNavigation<Views.OrderPageTablet, ViewModels.OrderPageTabletViewModel>();
            containerRegistry.RegisterForNavigation<Views.TabPageTablet, ViewModels.TabPageTabletViewModel>();
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

            await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(StartPage)}");
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

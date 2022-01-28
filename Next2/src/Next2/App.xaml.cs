using Next2.Resources.Strings;
using Next2.ViewModels;
using Mobile = Next2.Views.Mobile;
using Tablet = Next2.Views.Tablet;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;

namespace Next2
{
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null)
            : base(initializer)
        {
        }

        #region -- Overrides --

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();

            if (Device.Idiom == TargetIdiom.Phone)
            {
                containerRegistry.RegisterForNavigation<Mobile.MenuPage, MenuPageViewModel>();
            }
            else
            {
                containerRegistry.RegisterForNavigation<Tablet.MenuPage, MenuPageViewModel>();
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

            LocalizationResourceManager.Current.Init(Strings.ResourceManager);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(Mobile.MenuPage)}");
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
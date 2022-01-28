using InterTwitter.Services;
using InterTwitter.ViewModels;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Next2.Resources.Strings;
using Next2.Services.CustomersService;
using Next2.ViewModels;
using Next2.ViewModels.Dialogs;
using Next2.Views;
using Next2.Views.Mobile;
using Next2.Views.Mobile.Dialogs;
using Next2.Views.Tablet;
using Next2.Views.Tablet.Dialogs;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Prism.Unity;
using System;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            // Dialogs
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterPopupDialogService();
            containerRegistry.RegisterDialog<CustomerInfoDialogMob, CustomerInfoViewModel>();
            containerRegistry.RegisterDialog<CustomerInfoDialogTab, CustomerInfoViewModel>();
            //Services
            containerRegistry.RegisterSingleton<IMockService, MockService>();
            containerRegistry.RegisterSingleton<ICustomersService, CustomersService>();
            // Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<StartPage, StartPageViewModel>();
            containerRegistry.RegisterForNavigation<CustomersPageMob, CustomersPageViewModel>();
            containerRegistry.RegisterForNavigation<CustomersPageTab, CustomersPageViewModel>();
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

            await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(CustomersPageTab)}");
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

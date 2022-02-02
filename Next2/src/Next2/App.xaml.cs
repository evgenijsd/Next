using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Next2.ViewModels;
using Next2.ViewModels.Mobile;
using Next2.Views;
using Next2.Views.Tablet;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using System;
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
            // Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            if (Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            {
                containerRegistry.RegisterForNavigation<Views.Mobile.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterForNavigation<Views.Mobile.LoginPage_EmployeeId, LoginPage_EmployeeIdViewModel>();
            }
            else
            {
                containerRegistry.RegisterForNavigation<Views.Tablet.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterForNavigation<Views.StartPage, StartPageViewModel>();
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

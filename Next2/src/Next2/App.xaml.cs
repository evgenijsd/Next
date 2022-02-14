﻿using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Next2.Resources.Strings;
using Next2.Services;
using Next2.Services.Authentication;
using Next2.Services.ProfileService;
using Next2.Services.Services;
using Next2.ViewModels;
using Next2.Views.Tablet;
using Next2.Resources.Strings;
using Next2.Services.Membership;
using MobileViews = Next2.Views.Mobile;
using TabletViews = Next2.Views.Tablet;
using MobileViewModels = Next2.ViewModels.Mobile;
using TabletViewModels = Next2.ViewModels.Tablet;
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
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Next2.ViewModels.Tablet;
using Next2.Services.CustomersService;

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
            // Dialogs
            containerRegistry.RegisterPopupNavigationService();
            containerRegistry.RegisterPopupDialogService();
            //Services
            containerRegistry.RegisterSingleton<IMockService, MockService>();
            containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
            containerRegistry.RegisterSingleton<IUserService, UserService>();
            containerRegistry.RegisterSingleton<IAuthenticationService, AuthenticationService>();

            containerRegistry.RegisterSingleton<ICustomersService, CustomersService>();
            containerRegistry.RegisterSingleton<IMembershipService, MembershipService>();
            // Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();
            if (Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            {
                containerRegistry.RegisterForNavigation<MobileViews.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.LoginPage_EmployeeId, LoginPage_EmployeeIdViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.MenuPage, MobileViewModels.MenuPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.HoldItemsPage, HoldItemsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.OrderTabsPage, OrderTabsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.CustomersPage, CustomersViewModel>();
                containerRegistry.RegisterDialog<MobileViews.Dialogs.CustomerInfoDialog, CustomerInfoViewModel>();
                containerRegistry.RegisterDialog<MobileViews.Dialogs.CustomerAddDialog, CustomerInfoViewModel>();
            }
            else
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
                containerRegistry.RegisterDialog<TabletViews.Dialogs.CustomerInfoDialog, CustomerInfoViewModel>();
                containerRegistry.RegisterDialog<MobileViews.Dialogs.CustomerAddDialog, CustomerInfoViewModel>();
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
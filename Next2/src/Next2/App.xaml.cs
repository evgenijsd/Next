using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
using Next2.Services.Authentication;
using Next2.Services.Bonuses;
using Next2.Services.Customers;
using Next2.Services.Membership;
using Next2.Services.Menu;
using Next2.Services.Mock;
using Next2.Services.Order;
using Next2.Services.Rest;
using Next2.Services.Rewards;
using Next2.Services.Settings;
using Next2.Services.WorkLog;
using Next2.ViewModels;
using Next2.ViewModels.Mobile;
using Next2.ViewModels.Tablet;
using Next2.Views;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using Prism.Unity;
using System;
using System.Globalization;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using MobileViewModels = Next2.ViewModels.Mobile;
using MobileViews = Next2.Views.Mobile;
using TabletViewModels = Next2.ViewModels.Tablet;
using TabletViews = Next2.Views.Tablet;
using Next2.Services.Reservation;
using Next2.Services.Notifications;
using Next2.Views.Mobile;
using Prism.Navigation;
using Next2.Services.DishesHolding;

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

            // Services
            var mapper = CreateMapper();
            containerRegistry.RegisterInstance(mapper);
            containerRegistry.RegisterSingleton<ISettingsManager, SettingsManager>();
            containerRegistry.RegisterSingleton<IMockService, MockService>();
            containerRegistry.RegisterSingleton<INotificationsService, NotificationsService>();
            containerRegistry.RegisterSingleton<IRestService, RestService>();
            containerRegistry.RegisterSingleton<IAuthenticationService, AuthenticationService>();
            containerRegistry.RegisterSingleton<ICustomersService, CustomersService>();
            containerRegistry.RegisterSingleton<IOrderService, OrderService>();
            containerRegistry.RegisterSingleton<IRewardsService, RewardsService>();
            containerRegistry.RegisterSingleton<IMenuService, MenuService>();
            containerRegistry.RegisterSingleton<ICustomersService, CustomersService>();
            containerRegistry.RegisterSingleton<IBonusesService, BonusesService>();
            containerRegistry.RegisterSingleton<IWorkLogService, WorkLogService>();
            containerRegistry.RegisterSingleton<IReservationService, ReservationService>();
            containerRegistry.RegisterSingleton<IDishesHoldingService, DishesHoldingService>();

            // Navigation
            containerRegistry.RegisterForNavigation<NavigationPage>();

            if (IsTablet)
            {
                // Services
                containerRegistry.RegisterSingleton<IMembershipService, MembershipService>();

                //Navigation
                containerRegistry.RegisterForNavigation<TabletViews.SearchPage, SearchPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.BonusPage, BonusPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.TaxRemoveConfirmPage, TaxRemoveConfirmPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.MenuPage, TabletViewModels.MenuPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.ExpandPage, TabletViewModels.ExpandPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.PaymentPage, PaymentViewModel>();
                containerRegistry.RegisterForNavigation<InputTextPage, InputTextPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.ModificationsPage, ModificationsPageViewModel>();
                containerRegistry.RegisterForNavigation<TabletViews.SplitOrderPage, SplitOrderViewModel>();

                containerRegistry.RegisterSingleton<NewOrderViewModel>();
                containerRegistry.RegisterSingleton<HoldDishesViewModel>();
                containerRegistry.RegisterSingleton<OrderTabsViewModel>();
                containerRegistry.RegisterSingleton<ReservationsViewModel>();
                containerRegistry.RegisterSingleton<CustomersViewModel>();
                containerRegistry.RegisterSingleton<MembershipViewModel>();
                containerRegistry.RegisterSingleton<SettingsViewModel>();
                containerRegistry.RegisterSingleton<OrderRegistrationViewModel>();
                containerRegistry.RegisterSingleton<PrintReceiptViewModel>();
            }
            else
            {
                containerRegistry.RegisterForNavigation<MobileViews.LoginPage, LoginPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.BonusPage, BonusPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.EditPage, EditPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.LoginPage_EmployeeId, LoginPage_EmployeeIdViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.SettingsPage, SettingsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.MenuPage, MobileViewModels.MenuPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.HoldDishesPage, HoldDishesViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.OrderRegistrationPage, OrderRegistrationViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.OrderTabsPage, OrderTabsViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.CustomersPage, CustomersViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.ChooseDishPage, MobileViewModels.ChooseDishPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.SearchPage, SearchPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.PaymentPage, PaymentViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.OrderWithRewardsPage, OrderWithRewardsViewModel>();
                containerRegistry.RegisterForNavigation<InputTextPage, InputTextPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.ModificationsPage, ModificationsPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.TipsPage, TipsPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.InputCashPage, InputCashPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.InputGiftCardPage, InputGiftCardPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.WaitingSwipeCardPage, WaitingSwipeCardPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.WaitingSignaturePage, WaitingSignaturePageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.TaxRemoveConfirmPage, TaxRemoveConfirmPageViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.SplitOrderPage, SplitOrderViewModel>();
                containerRegistry.RegisterForNavigation<MobileViews.PrintReceiptPage, PrintReceiptViewModel>();
            }
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();
            App.Current.UserAppTheme = OSAppTheme.Dark;

            LocalizationResourceManager.Current.Init(Strings.ResourceManager);

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            var navigationParameters = new NavigationParameters();
            var navigationPath = $"{nameof(NavigationPage)}/";

            IAuthenticationService authenticationService = Resolve<IAuthenticationService>();

            if (authenticationService.IsAuthorizationComplete)
            {
                navigationParameters.Add(Constants.Navigations.IS_FIRST_ORDER_INIT, true);
                navigationPath += nameof(MenuPage);
            }
            else
            {
                navigationPath += nameof(LoginPage);
            }

            await NavigationService.NavigateAsync(navigationPath, navigationParameters);
        }

        protected override void OnStart()
        {
#if !DEBUG
            AppCenter.Start(
                $"ios={Constants.Analytics.IOSKey};android={Constants.Analytics.AndroidKey};",
                typeof(Analytics),
                typeof(Crashes));

            Analytics.SetEnabledAsync(true);
#endif
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        #endregion

        #region -- Public static helpers --

        public static T Resolve<T>() => Current.Container.Resolve<T>();

        #endregion

        #region -- Private helpers --

        private IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TableModelDTO, TableBindableModel>()
                    .ForMember(x => x.TableNumber, s => s.MapFrom(x => x.Number));
                cfg.CreateMap<CustomerModelDTO, CustomerBindableModel>().ReverseMap();
                cfg.CreateMap<CustomerModelDTO, UpdateCustomerCommand>();
                cfg.CreateMap<RewardModel, RewardBindabledModel>();
                cfg.CreateMap<MemberBindableModel, MemberBindableModel>();
                cfg.CreateMap<DiscountModelDTO, BonusBindableModel>().ReverseMap();
                cfg.CreateMap<CouponModelDTO, BonusBindableModel>().ReverseMap();
                cfg.CreateMap<OrderModelDTO, FullOrderBindableModel>()
                    .ForMember(x => x.OrderType, x => x.MapFrom(s => (EOrderType)Enum.Parse(typeof(EOrderType), s.OrderType)));
                cfg.CreateMap<FullOrderBindableModel, FullOrderBindableModel>();
                cfg.CreateMap<SeatBindableModel, SeatBindableModel>();
                cfg.CreateMap<DishBindableModel, DishBindableModel>();
                cfg.CreateMap<ProductBindableModel, ProductBindableModel>()
                    .AfterMap((s, d) => d.Product = s.Product);
                cfg.CreateMap<MembershipModelDTO, MemberBindableModel>();
                cfg.CreateMap<MemberBindableModel, MembershipModelDTO>();
                cfg.CreateMap<OptionModelDTO, OptionModelDTO>();
                cfg.CreateMap<SimpleIngredientModelDTO, SimpleIngredientModelDTO>();
                cfg.CreateMap<SimpleIngredientsCategoryModelDTO, SimpleIngredientsCategoryModelDTO>();
                cfg.CreateMap<TableBindableModel, SimpleTableModelDTO>()
                    .ForMember(x => x.Number, s => s.MapFrom(x => x.TableNumber));
                cfg.CreateMap<DishModelDTO, DishBindableModel>();
                cfg.CreateMap<SimpleIngredientsCategoryModelDTO, IngredientsCategoryModelDTO>();
                cfg.CreateMap<ProductBindableModel, SimpleProductModelDTO>().ReverseMap();
                cfg.CreateMap<GiftCardModelDTO, UpdateGiftCardCommand>().ReverseMap();
                cfg.CreateMap<SimpleProductModelDTO, SimpleProductModelDTO>();
                cfg.CreateMap<GiftCardModelDTO, GiftCardModelDTO>();
                cfg.CreateMap<PaidOrderBindableModel, PaidOrderBindableModel>();
                cfg.CreateMap<HoldDishModel, HoldDishBindableModel>().ReverseMap();
                cfg.CreateMap<HoldDishBindableModel, TableBindableModel>()
                    .ForMember(x => x.Id, s => s.Ignore());
                cfg.CreateMap<SimpleOrderModelDTO, SimpleOrderBindableModel>()
                        .ForMember<string>(x => x.TableNumberOrName, s => s.MapFrom(x => GetTableName(x.TableNumber)));
            }).CreateMapper();
        }

        private string GetTableName(int? tableNumber)
        {
            return tableNumber == null
                ? LocalizationResourceManager.Current["NotDefined"]
                : $"{LocalizationResourceManager.Current["Table"]} {tableNumber}";
        }

        #endregion
    }
}
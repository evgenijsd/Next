using Next2.Resources.Strings;
using Next2.ViewModels;
using Next2.Views;
using Prism;
using Prism.Ioc;
using Prism.Unity;
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
            containerRegistry.RegisterForNavigation<StartPage, StartPageViewModel>();
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            LocalizationResourceManager.Current.Init(Strings.ResourceManager);

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

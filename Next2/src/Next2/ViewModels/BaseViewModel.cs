using Next2.Interfaces;
using Next2.Services.Authentication;
using Next2.Services.Notifications;
using Next2.Views.Mobile;
using Prism.Mvvm;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class BaseViewModel : BindableBase, IDestructible, IInitialize, IInitializeAsync, INavigationAware, IPageActionsHandler
    {
        protected INavigationService _navigationService { get; }

        protected IAuthenticationService _authenticationService { get; }

        protected INotificationsService _notificationsService { get; }

        protected bool IsInternetConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public BaseViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService)
        {
            _navigationService = navigationService;
            _authenticationService = authenticationService;
            _notificationsService = notificationsService;
        }

        #region -- Public properties --

        private IPopupNavigation _popupNavigation;
        public IPopupNavigation PopupNavigation => _popupNavigation ??= App.Resolve<IPopupNavigation>();

        #endregion

        #region -- IDestructible implementation --

        public virtual void Destroy()
        {
        }

        #endregion

        #region -- IInitialize implementation --

        public virtual void Initialize(INavigationParameters parameters)
        {
        }

        #endregion

        #region -- IInitializeAsync implementation --

        public virtual Task InitializeAsync(INavigationParameters parameters)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region -- INavigationAware implementation --

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        #endregion

        #region -- IPageActionsHandler implementation --

        public virtual void OnAppearing()
        {
        }

        public virtual void OnDisappearing()
        {
        }

        #endregion

        #region -- Public helpers --

        public Task ResponseToUnsuccessfulRequestAsync(string? statusCode)
        {
            return statusCode == Constants.StatusCode.UNAUTHORIZED
                ? PerformLogoutAsync()
                : _notificationsService.ResponseToBadRequestAsync(statusCode);
        }

        public async Task PerformLogoutAsync()
        {
            _authenticationService.ClearSession();

            var navigationParameters = new NavigationParameters
            {
                { Constants.Navigations.LOGOUT, true },
            };

            Device.BeginInvokeOnMainThread(async () =>
            {
                await _navigationService.NavigateAsync($"/{nameof(NavigationPage)}/{nameof(LoginPage)}", navigationParameters);
            });
        }

        #endregion
    }
}
using Next2.Interfaces;
using Next2.Services.Authentication;
using Next2.Views.Mobile;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class BaseViewModel : BindableBase, IDestructible, IInitialize, IInitializeAsync, INavigationAware, IPageActionsHandler
    {
        protected INavigationService _navigationService { get; }

        protected bool IsInternetConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public BaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        #region -- Public properties --

        private IPopupNavigation _popupNavigation;
        public IPopupNavigation PopupNavigation => _popupNavigation ??= App.Resolve<IPopupNavigation>();

        #endregion

        #region -- Protected helpers --

        protected async Task ResponseToBadRequestAsync(string statusCode)
        {
            if (statusCode == Constants.StatusCode.UNAUTHORIZED)
            {
                var authenticationService = App.Resolve<IAuthenticationService>();

                authenticationService.ClearSession();

                var navigationParameters = new NavigationParameters
                {
                    { Constants.Navigations.LOGOUT, true },
                };

                await _navigationService.NavigateAsync($"{nameof(LoginPage)}", navigationParameters);
            }
            else if (statusCode == Constants.StatusCode.BAD_REQUEST)
            {
                await ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["SomethingWentWrong"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        protected Task ShowInfoDialogAsync(string titleText, string descriptionText, string okText)
        {
            var parameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.TITLE, titleText },
                { Constants.DialogParameterKeys.DESCRIPTION,  descriptionText },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, okText },
            };

            PopupPage infoDialog = App.IsTablet
                ? new Views.Tablet.Dialogs.InfoDialog(parameters, () => PopupNavigation.PopAsync())
                : new Views.Mobile.Dialogs.InfoDialog(parameters, () => PopupNavigation.PopAsync());

            return PopupNavigation.PushAsync(infoDialog);
        }

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
    }
}
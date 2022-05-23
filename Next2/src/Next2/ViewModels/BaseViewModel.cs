using Next2.Interfaces;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Essentials;

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

        #region -- Private helpers --

        protected Task ShowInfoDialog(string titleTextKey, string descriptionTextKey, string closeTextKey = "Ok")
        {
            var parameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current[titleTextKey] },
                { Constants.DialogParameterKeys.DESCRIPTION,  LocalizationResourceManager.Current[descriptionTextKey] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current[closeTextKey] },
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
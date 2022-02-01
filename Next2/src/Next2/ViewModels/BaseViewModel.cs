using Next2.Interfaces;
using Prism.Mvvm;
using Prism.Navigation;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Next2.ViewModels
{
    public class BaseViewModel : BindableBase, IDestructible, IInitialize, IInitializeAsync, INavigationAware, IPageActionsHandler
    {
        protected INavigationService _navigationService { get; }

        protected bool IsConnectionExist
        {
            get
            {
                var connectivity = Connectivity.NetworkAccess;
                return connectivity != NetworkAccess.None && connectivity != NetworkAccess.Unknown;
            }
        }

        public BaseViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

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
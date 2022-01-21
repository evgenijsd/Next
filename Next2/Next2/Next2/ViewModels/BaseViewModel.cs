using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Essentials;

namespace Next2.ViewModels
{
    public class BaseViewModel : BindableBase, IDestructible, IInitialize, INavigationAware
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

        #region -- INavigationAware implementation --

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {
        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {
        }

        #endregion
    }
}
using Next2.Views;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class StartPageViewModel : BaseViewModel
    {
        public StartPageViewModel(INavigationService navigationService)
           : base(navigationService)
        {
        }

        #region -- Public properties --

        private ICommand _OrderCommand;

        public ICommand OrderCommand => _OrderCommand ??= new AsyncCommand(OnOrderCommandAsync);

        private ICommand _TabCommand;

        public ICommand TabCommand => _TabCommand ??= new AsyncCommand(OnTabCommandAsync);

        #endregion

        #region -- Private helpers --

        private async Task OnOrderCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(OrderPageTablet)}");
        }

        private async Task OnTabCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(TabPageMobile)}");
        }

        public ICommand CrashCommand => new Command(() => throw new NullReferenceException());

        #endregion

    }
}

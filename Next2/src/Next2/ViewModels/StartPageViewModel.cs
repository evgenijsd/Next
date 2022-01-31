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

        private bool _isSelectedOrders = true;

        public bool IsSelectedOrders
        {
            get => _isSelectedOrders;
            set => SetProperty(ref _isSelectedOrders, value);
        }

        private bool _isSelectedTabs = false;

        public bool IsSelectedTabs
        {
            get => _isSelectedTabs;
            set => SetProperty(ref _isSelectedTabs, value);
        }

        private ICommand _ButtonOrdersCommand;

        public ICommand ButtonOrdersCommand => _ButtonOrdersCommand ??= new AsyncCommand(OnButtonOrdersCommandAsync);

        private ICommand _ButtonTabsCommand;

        public ICommand ButtonTabsCommand => _ButtonTabsCommand ??= new AsyncCommand(OnButtonTabsCommandAsync);

        private ICommand _OrderCommand;

        public ICommand OrderCommand => _OrderCommand ??= new AsyncCommand(OnOrderCommandAsync);

        private ICommand _TabCommand;

        public ICommand TabCommand => _TabCommand ??= new AsyncCommand(OnTabCommandAsync);

        #endregion

        #region -- Private helpers --

        private async Task OnOrderCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(OrderTabPageMobile)}");
        }

        private async Task OnTabCommandAsync()
        {
            await _navigationService.NavigateAsync($"{nameof(TabPageMobile)}");
        }

        public ICommand CrashCommand => new Command(() => throw new NullReferenceException());

        private Task OnButtonOrdersCommandAsync()
        {
            if (IsSelectedTabs)
            {
                IsSelectedOrders = !IsSelectedOrders;
                IsSelectedTabs = !IsSelectedTabs;
            }

            return Task.CompletedTask;
        }

        private Task OnButtonTabsCommandAsync()
        {
            if (IsSelectedOrders)
            {
                IsSelectedOrders = !IsSelectedOrders;
                IsSelectedTabs = !IsSelectedTabs;
            }

            return Task.CompletedTask;
        }

        #endregion

    }
}

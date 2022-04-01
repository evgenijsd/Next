using Next2.Models;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class OrderWithRewardsViewModel : BaseViewModel
    {
        public OrderWithRewardsViewModel(
            INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        public ObservableCollection<SeatWithDiscountedBindableModel> Seats { get; set; } = new();

        private ICommand _applyRewardCommand;
        public ICommand ApplyRewardCommand => _applyRewardCommand ??= new AsyncCommand(OnApplyRewardCommandAsync);

        #endregion
        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.SEATS, out ObservableCollection<SeatWithDiscountedBindableModel> seats))
            {
                Seats = seats;
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnApplyRewardCommandAsync()
        {
            var parameters = new NavigationParameters
            {
                { Constants.Navigations.SEATS, Seats },
            };

            return _navigationService.GoBackAsync(parameters);
        }

        #endregion
    }
}

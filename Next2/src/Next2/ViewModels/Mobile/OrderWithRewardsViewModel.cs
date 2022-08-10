using Next2.Models;
using Prism.Navigation;
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

        public RewardBindabledModel Reward { get; set; } = new();

        public ObservableCollection<SeatWithFreeDishesBindableModel> Seats { get; set; } = new();

        private ICommand? _applyRewardCommand;
        public ICommand ApplyRewardCommand => _applyRewardCommand ??= new AsyncCommand(OnApplyRewardCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.REWARD, out RewardBindabledModel reward) &&
                parameters.TryGetValue(Constants.Navigations.SEATS, out ObservableCollection<SeatWithFreeDishesBindableModel> seats))
            {
                Reward = reward;
                Seats = seats;
            }
        }

        public override void OnNavigatedFrom(INavigationParameters parameters)
        {
            parameters.Add(Constants.Navigations.REWARD, Reward);

            base.OnNavigatedFrom(parameters);
        }

        #endregion

        #region -- Private helpers --

        private Task OnApplyRewardCommandAsync()
        {
            var parameters = new NavigationParameters
            {
                { Constants.Navigations.IS_REWARD_APPLIED, true },
                { Constants.Navigations.SEATS, Seats },
                { Constants.Navigations.CONFIRMED_APPLY_REWARD, true },
            };

            return _navigationService.GoBackAsync(parameters);
        }

        #endregion
    }
}

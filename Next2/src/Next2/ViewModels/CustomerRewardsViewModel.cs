using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Services.CustomersService;
using Next2.Services.Order;
using Next2.Services.Rewards;
using Next2.Views.Mobile;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class CustomerRewardsViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly ICustomersService _customersService;
        private readonly IRewardsService _rewardService;

        public CustomerRewardsViewModel(
            INavigationService navigationService,
            IMapper mapper,
            IOrderService orderService,
            ICustomersService customersService,
            IRewardsService rewardService)
            : base(navigationService)
        {
            _mapper = mapper;
            _orderService = orderService;
            _customersService = customersService;
            _rewardService = rewardService;
        }

        #region -- Public properties --

        public string CustomerName { get; set; }

        public bool IsRewardSelectionCompleted { get; set; }

        public bool IsAnyRewardsApplied { get; set; }

        public ObservableCollection<RewardBindabledModel> Rewards { get; set; } = new ();

        public ObservableCollection<SeatWithFreeSetsBindableModel> Seats { get; set; } = new ();

        public ECustomerRewardsPageState PageState { get; set; }

        private ICommand _addNewCustomerCommand;
        public ICommand AddNewCustomerCommand => _addNewCustomerCommand ??= new AsyncCommand(OnAddNewCustomerCommandAsync, allowsMultipleExecutions: false);

        private ICommand _selectRewardCommand;
        public ICommand SelectRewardCommand => _selectRewardCommand ??= new AsyncCommand<RewardBindabledModel>(OnSelectRewardCommandAsync, allowsMultipleExecutions: false);

        private ICommand _goToCompleteTabCommand;
        public ICommand GoToCompleteTabCommand => _goToCompleteTabCommand ??= new AsyncCommand(OnGoToCompleteTabCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (App.IsTablet)
            {
                var customer = _orderService.CurrentOrder.Customer;
                await LoadPageData(customer);
            }
            else if (
                parameters.TryGetValue(Constants.Navigations.IS_REWARD_APPLIED, out bool isRewardApplied)
                && parameters.TryGetValue(Constants.Navigations.REWARD, out RewardBindabledModel reward))
            {
                reward.IsApplied = isRewardApplied;

                if (isRewardApplied && parameters.TryGetValue(Constants.Navigations.SEATS, out ObservableCollection<SeatWithFreeSetsBindableModel> seats))
                {
                    Seats = seats;
                    LockUnlockSimilarRewards(Seats, reward);
                }
                else
                {
                    ApplyCancelRewardToSet(Seats, reward);
                }
            }
            else
            {
                var customer = _orderService.CurrentOrder.Customer;
                await LoadPageData(customer);
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task LoadPageData(CustomerModel customer)
        {
            if (customer.Name == string.Empty)
            {
                PageState = ECustomerRewardsPageState.NoSelectedCustomer;
            }
            else
            {
                CustomerName = customer.Name;

                var customersRewardsResult = await _rewardService.GetCustomersRewards(customer.Id);

                if (!customersRewardsResult.IsSuccess)
                {
                    PageState = ECustomerRewardsPageState.RewardsNotExist;
                }
                else
                {
                    PageState = ECustomerRewardsPageState.RewardsExist;

                    await LoadSeats();

                    Rewards = _mapper.Map<IEnumerable<RewardModel>, ObservableCollection<RewardBindabledModel>>(customersRewardsResult.Result);

                    foreach (var reward in Rewards)
                    {
                        reward.SelectCommand = SelectRewardCommand;
                        reward.IsCanBeApplied = Seats.Any(x => x.Sets.Any(x => x.Id == reward.SetId));
                    }
                }
            }
        }

        public Task LoadSeats()
        {
            var seats = _orderService.CurrentOrder.Seats.Where(x => x.Sets.Any());

            foreach (var seat in seats)
            {
                var sets = _mapper.Map<ObservableCollection<SetBindableModel>, ObservableCollection<FreeSetBindableModel>>(seat.Sets);

                var newSeat = new SeatWithFreeSetsBindableModel
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    Sets = sets,
                };

                Seats.Add(newSeat);
            }

            return Task.CompletedTask;
        }

        private void ApplyCancelRewardToSet(ObservableCollection<SeatWithFreeSetsBindableModel> seats, RewardBindabledModel reward)
        {
            Func<FreeSetBindableModel, bool> setComparer = y => y.Id == reward.SetId && y.IsFree != reward.IsApplied;

            var seat = reward.IsApplied
                ? seats.FirstOrDefault(x => x.Sets.Any(setComparer))
                : seats.LastOrDefault(x => x.Sets.Any(setComparer));

            if (seat is not null)
            {
                var set = reward.IsApplied
                     ? seat.Sets.FirstOrDefault(setComparer)
                     : seat.Sets.LastOrDefault(setComparer);

                set.IsFree = reward.IsApplied;
                seat.Sets = new(seat.Sets);
            }
        }

        public void LockUnlockSimilarRewards(ObservableCollection<SeatWithFreeSetsBindableModel> seats, RewardBindabledModel selectedReward)
        {
            foreach (var reward in Rewards)
            {
                if (!reward.IsApplied && reward.SetId == selectedReward.SetId)
                {
                    reward.IsCanBeApplied = seats.Any(seat => seat.Sets.Any(set => set.Id == selectedReward.SetId && !set.IsFree));
                }
            }
        }

        private async Task OnSelectRewardCommandAsync(RewardBindabledModel selectedReward)
        {
            if (selectedReward is not null)
            {
                if (App.IsTablet)
                {
                    selectedReward.IsApplied = !selectedReward.IsApplied;
                    ApplyCancelRewardToSet(Seats, selectedReward);
                    LockUnlockSimilarRewards(Seats, selectedReward);

                    IsAnyRewardsApplied = Rewards.Any(x => x.IsApplied);
                }
                else
                {
                    if (selectedReward.IsApplied)
                    {
                        selectedReward.IsApplied = false;
                        ApplyCancelRewardToSet(Seats, selectedReward);
                        LockUnlockSimilarRewards(Seats, selectedReward);
                    }
                    else
                    {
                        ObservableCollection<SeatWithFreeSetsBindableModel> setsForRewardApplying = new (Seats);

                        selectedReward.IsApplied = !selectedReward.IsApplied;
                        ApplyCancelRewardToSet(setsForRewardApplying, selectedReward);

                        var parameters = new NavigationParameters
                        {
                            { Constants.Navigations.REWARD, selectedReward },
                            { Constants.Navigations.SEATS, setsForRewardApplying },
                        };

                        await _navigationService.NavigateAsync(nameof(OrderWithRewardsPage), parameters);
                    }
                }
            }
        }

        private async Task OnAddNewCustomerCommandAsync()
        {
            // temporary code
            var customersResult = await _customersService.GetAllCustomersAsync();

            if (customersResult.IsSuccess)
            {
                var customer = customersResult.Result.LastOrDefault();
                _orderService.CurrentOrder.Customer = customer;

                await LoadPageData(customer);
            }
        }

        private Task OnGoToCompleteTabCommandAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}
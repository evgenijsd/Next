﻿using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
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
    public class RewardsViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly ICustomersService _customersService;
        private readonly IRewardsService _rewardService;

        public RewardsViewModel(
            INavigationService navigationService,
            IMapper mapper,
            IOrderService orderService,
            ICustomersService customersService,
            IRewardsService rewardService,
            Action<NavigationMessage> navigateAsync,
            Action<EPaymentPageSteps> goToCompleteStep)
            : base(navigationService)
        {
            _mapper = mapper;
            _orderService = orderService;
            _customersService = customersService;
            _rewardService = rewardService;

            NavigateAsync = navigateAsync;
            GoToStep = goToCompleteStep;
        }

        #region -- Public properties --

        public ERewardsPageState PageState { get; set; }

        public string CustomerName { get; set; }

        public bool IsRefreshing { get; set; }

        public bool IsAnyRewardsApplied { get; set; }

        public ObservableCollection<RewardBindabledModel> Rewards { get; set; } = new ();

        public ObservableCollection<SeatWithFreeSetsBindableModel> Seats { get; set; } = new ();

        private Action<NavigationMessage> NavigateAsync;

        private Action<EPaymentPageSteps> GoToStep;

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

            if (parameters.TryGetValue(Constants.Navigations.IS_REWARD_APPLIED, out bool isRewardApplied)
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
                await LoadPageDataAsync(customer);
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task LoadPageDataAsync(CustomerModel customer)
        {
            IsRefreshing = true;

            if (customer.Name == string.Empty)
            {
                PageState = ERewardsPageState.NoSelectedCustomer;
            }
            else
            {
                CustomerName = customer.Name;

                var customersRewardsResult = await _rewardService.GetCustomersRewards(customer.Id);

                if (!customersRewardsResult.IsSuccess)
                {
                    PageState = ERewardsPageState.RewardsNotExist;
                }
                else
                {
                    PageState = ERewardsPageState.RewardsExist;

                    await LoadSeatsAsync();

                    Rewards = _mapper.Map<IEnumerable<RewardModel>, ObservableCollection<RewardBindabledModel>>(customersRewardsResult.Result, opt => opt.AfterMap((input, output) =>
                    {
                        foreach (var reward in output)
                        {
                            reward.SelectCommand = SelectRewardCommand;
                            reward.CanBeApplied = Seats.Any(x => x.Sets.Any(x => x.Id == reward.SetId));
                        }
                    }));
                }
            }

            IsRefreshing = false;
        }

        public Task LoadSeatsAsync()
        {
            Seats.Clear();

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
                seat.Sets = new (seat.Sets);
            }

            IsAnyRewardsApplied = Rewards.Any(x => x.IsApplied);
        }

        public void LockUnlockSimilarRewards(ObservableCollection<SeatWithFreeSetsBindableModel> seats, RewardBindabledModel selectedReward)
        {
            foreach (var reward in Rewards)
            {
                if (!reward.IsApplied && reward.SetId == selectedReward.SetId)
                {
                    reward.CanBeApplied = seats.Any(seat => seat.Sets.Any(set => set.Id == selectedReward.SetId && !set.IsFree));
                }
            }
        }

        private Task OnSelectRewardCommandAsync(RewardBindabledModel? selectedReward)
        {
            if (selectedReward is not null)
            {
                if (App.IsTablet)
                {
                    selectedReward.IsApplied = !selectedReward.IsApplied;
                    ApplyCancelRewardToSet(Seats, selectedReward);
                    LockUnlockSimilarRewards(Seats, selectedReward);
                }
                else if (selectedReward.IsApplied)
                {
                    selectedReward.IsApplied = false;
                    ApplyCancelRewardToSet(Seats, selectedReward);
                    LockUnlockSimilarRewards(Seats, selectedReward);
                }
                else
                {
                    ObservableCollection<SeatWithFreeSetsBindableModel> setsForRewardApplying = new (Seats);

                    selectedReward.IsApplied = true;
                    ApplyCancelRewardToSet(setsForRewardApplying, selectedReward);

                    var parameters = new NavigationParameters
                    {
                        { Constants.Navigations.REWARD, selectedReward },
                        { Constants.Navigations.SEATS, setsForRewardApplying },
                    };

                    NavigateAsync(new NavigationMessage(nameof(OrderWithRewardsPage), parameters));
                }
            }

            return Task.CompletedTask;
        }

        private async Task OnAddNewCustomerCommandAsync()
        {
            // temporary code
            var customersResult = await _customersService.GetAllCustomersAsync();

            if (customersResult.IsSuccess)
            {
                var customer = customersResult.Result.LastOrDefault();
                _orderService.CurrentOrder.Customer = customer;

                await LoadPageDataAsync(customer);
            }
        }

        private Task OnGoToCompleteTabCommandAsync()
        {
            GoToStep(EPaymentPageSteps.Complete);

            return Task.CompletedTask;
        }

        #endregion
    }
}
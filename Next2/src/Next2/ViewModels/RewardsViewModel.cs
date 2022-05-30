using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.CustomersService;
using Next2.Services.Order;
using Next2.Services.Rewards;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
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
        private readonly IPopupNavigation _popupNavigation;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly ICustomersService _customersService;
        private readonly IRewardsService _rewardService;

        public RewardsViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            IMapper mapper,
            IOrderService orderService,
            ICustomersService customersService,
            IRewardsService rewardService,
            PaidOrderBindableModel order,
            Action<NavigationMessage> navigateAsync,
            Action<EPaymentSteps> goToPaymentStep)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;
            _mapper = mapper;
            _orderService = orderService;
            _customersService = customersService;
            _rewardService = rewardService;

            Order = order;
            NavigateAsync = navigateAsync;
            GoToPaymentStep = goToPaymentStep;
        }

        #region -- Public properties --

        public ERewardsPageState PageState { get; set; }

        public PaidOrderBindableModel Order { get; set; }

        private readonly Action<NavigationMessage> NavigateAsync;

        private readonly Action<EPaymentSteps> GoToPaymentStep;

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

            if (!App.IsTablet)
            {
                if (parameters.TryGetValue(Constants.Navigations.IS_REWARD_APPLIED, out bool isRewardApplied)
                    && parameters.TryGetValue(Constants.Navigations.REWARD, out RewardBindabledModel reward)
                     && parameters.TryGetValue(Constants.Navigations.CONFIRMED_APPLY_REWARD, out bool isRewardConfirmedAppy))
                {
                    reward.IsApplied = isRewardApplied;
                    reward.IsConfirmedApply = isRewardConfirmedAppy;

                    if (isRewardApplied && parameters.TryGetValue(Constants.Navigations.SEATS, out ObservableCollection<SeatWithFreeSetsBindableModel> seats))
                    {
                        Order.Seats = seats;
                        LockUnlockSimilarRewards(Order.Seats, reward);
                    }
                    else
                    {
                        ApplyCancelRewardToSet(Order.Seats, reward);
                    }
                }
                else
                {
                    await RefreshPageDataAsync();
                }
            }
            else
            {
                await RefreshPageDataAsync();
            }
        }

        #endregion

        #region -- Private helpers --

        public async Task RefreshPageDataAsync()
        {
            Order.Id = _orderService.CurrentOrder.Id;
            Order.Customer = new(); // _orderService.CurrentOrder.Customer;

            if (Order.Customer is null)
            {
                PageState = ERewardsPageState.NoSelectedCustomer;
            }
            else
            {
                var customersRewardsResult = await _rewardService.GetCustomersRewards(Order.Customer.Id);

                if (!customersRewardsResult.IsSuccess)
                {
                    PageState = ERewardsPageState.RewardsNotExist;
                }
                else
                {
                    PageState = ERewardsPageState.RewardsExist;

                    LoadSeats();

                    Order.Rewards = _mapper.Map<IEnumerable<RewardModel>, ObservableCollection<RewardBindabledModel>>(customersRewardsResult.Result, opt => opt.AfterMap((input, output) =>
                    {
                        foreach (var reward in output)
                        {
                            reward.SelectCommand = SelectRewardCommand;
                            reward.CanBeApplied = Order.Seats.Any(x => x.Sets.Any(x => x.Id == reward.SetId));
                        }
                    }));
                }
            }
        }

        public void LoadSeats()
        {
            //var bindableSeats = _orderService.CurrentOrder.Seats.Where(x => x.SelectedDishes.Any());

            //Order.Seats.Clear();

            //foreach (var seat in bindableSeats)
            //{
            //    var freeSets = _mapper.Map<ObservableCollection<FreeSetBindableModel>>(seat.SelectedDishes);

            //    if (App.IsTablet)
            //    {
            //        SetProductsNamesForSets(seat.SelectedDishes, freeSets);
            //    }

            //    var newSeat = new SeatWithFreeSetsBindableModel
            //    {
            //        Id = seat.Id,
            //        SeatNumber = seat.SeatNumber,
            //        Sets = freeSets,
            //    };

            //    Order.Seats.Add(newSeat);
            //}
        }

        private void SetProductsNamesForSets(ObservableCollection<SetBindableModel> setBindables, ObservableCollection<FreeSetBindableModel> freeSets)
        {
            //for (int i = 0; i < setBindables.Count; i++)
            //{
            //    freeSets[i].ProductNames = string.Join(", ", setBindables[i].Products.Select(x => x.Title));
            //}
        }

        private void ApplyCancelRewardToSet(ObservableCollection<SeatWithFreeSetsBindableModel> seats, RewardBindabledModel reward)
        {
            bool CompareSetWithRewardSet(FreeSetBindableModel set) => set.Id == reward.SetId && set.IsFree != reward.IsApplied;

            var seat = reward.IsApplied
                ? seats.FirstOrDefault(x => x.Sets.Any(CompareSetWithRewardSet))
                : seats.LastOrDefault(x => x.Sets.Any(CompareSetWithRewardSet));

            if (seat is not null)
            {
                var set = reward.IsApplied
                     ? seat.Sets.FirstOrDefault(CompareSetWithRewardSet)
                     : seat.Sets.LastOrDefault(CompareSetWithRewardSet);

                set.IsFree = reward.IsApplied;
                seat.Sets = new (seat.Sets);
            }

            Order.IsUnsavedChangesExist = Order.Rewards.Any(x => x.IsApplied);
        }

        public void LockUnlockSimilarRewards(ObservableCollection<SeatWithFreeSetsBindableModel> seats, RewardBindabledModel selectedReward)
        {
            foreach (var reward in Order.Rewards)
            {
                if (!reward.IsApplied && reward.SetId == selectedReward.SetId)
                {
                    reward.CanBeApplied = seats.Any(seat => seat.Sets.Any(set => set.Id == selectedReward.SetId && !set.IsFree));
                }
            }
        }

        private async void AddNewCustomerDialogCallBackAsync(IDialogParameters parameters)
        {
            await _popupNavigation.PopAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.CUSTOMER_ID, out Guid newCustomerId))
            {
                var customerResult = await _customersService.GetAllCustomersAsync(x => x.Id == newCustomerId);

                if (customerResult.IsSuccess)
                {
                    _orderService.CurrentOrder.Customer = customerResult.Result.FirstOrDefault();

                    await RefreshPageDataAsync();
                }
            }
        }

        private Task OnAddNewCustomerCommandAsync()
        {
            var param = new DialogParameters();

            PopupPage popupPage = App.IsTablet
                ? new Views.Tablet.Dialogs.CustomerAddDialog(param, AddNewCustomerDialogCallBackAsync, _customersService)
                : new Views.Mobile.Dialogs.CustomerAddDialog(param, AddNewCustomerDialogCallBackAsync, _customersService);

            return _popupNavigation.PushAsync(popupPage);
        }

        private Task OnSelectRewardCommandAsync(RewardBindabledModel? selectedReward)
        {
            if (selectedReward is not null)
            {
                if (App.IsTablet)
                {
                    selectedReward.IsApplied = !selectedReward.IsApplied;
                    ApplyCancelRewardToSet(Order.Seats, selectedReward);
                    LockUnlockSimilarRewards(Order.Seats, selectedReward);
                }
                else if (selectedReward.IsApplied)
                {
                    selectedReward.IsApplied = false;
                    selectedReward.IsConfirmedApply = false;
                    ApplyCancelRewardToSet(Order.Seats, selectedReward);
                    LockUnlockSimilarRewards(Order.Seats, selectedReward);
                }
                else
                {
                    ObservableCollection<SeatWithFreeSetsBindableModel> seatsForRewardApplying = new (Order.Seats);

                    selectedReward.IsApplied = true;
                    ApplyCancelRewardToSet(seatsForRewardApplying, selectedReward);

                    var parameters = new NavigationParameters
                    {
                        { Constants.Navigations.REWARD, selectedReward },
                        { Constants.Navigations.SEATS, seatsForRewardApplying },
                    };

                    NavigateAsync(new NavigationMessage(nameof(OrderWithRewardsPage), parameters));
                }
            }

            return Task.CompletedTask;
        }

        private Task OnGoToCompleteTabCommandAsync()
        {
            GoToPaymentStep(EPaymentSteps.Complete);

            return Task.CompletedTask;
        }

        #endregion
    }
}
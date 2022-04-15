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
            PaidOrderBindableModel orderForPayment,
            Action<NavigationMessage> navigateAsync,
            Action<EPaymentPageSteps> goToPaymentStep)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;
            _mapper = mapper;
            _orderService = orderService;
            _customersService = customersService;
            _rewardService = rewardService;

            Order = orderForPayment;
            NavigateAsync = navigateAsync;
            GoToPaymentStep = goToPaymentStep;
        }

        #region -- Public properties --

        public ERewardsPageState PageState { get; set; }

        public PaidOrderBindableModel Order { get; set; }

        private readonly Action<NavigationMessage> NavigateAsync;

        private readonly Action<EPaymentPageSteps> GoToPaymentStep;

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
                    && parameters.TryGetValue(Constants.Navigations.REWARD, out RewardBindabledModel reward))
                {
                    reward.IsApplied = isRewardApplied;

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

        #region -- Public helpers --

        public async Task RefreshPageDataAsync()
        {
            if (Order.IsOrderEditing)
            {
                Order.Customer = _orderService.CurrentOrder.Customer;
            }
            else
            {
                var orderResult = await _orderService.GetOrderByIdAsync(Order.Id);

                if (orderResult.IsSuccess && orderResult.Result.Customer.Id != 0)
                {
                    Order.Customer = orderResult.Result.Customer;
                }
            }

            if (Order.Customer.Id == 0)
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

                    await LoadSeats();

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

        #endregion

        #region -- Private helpers --

        public async Task LoadSeats()
        {
            IEnumerable<SeatModel> seats = new List<SeatModel>();

            if (Order.IsOrderEditing)
            {
                var bindableSeats = _orderService.CurrentOrder.Seats.Where(x => x.Sets.Any());

                seats = _mapper.Map<IEnumerable<SeatModel>>(bindableSeats);
            }
            else
            {
                var seatsResult = await _orderService.GetSeatsAsync(Order.Id);

                if (seatsResult.IsSuccess)
                {
                    seats = seatsResult.Result;
                }
            }

            Order.Seats.Clear();

            foreach (var seat in seats)
            {
                var freeSets = _mapper.Map<List<SetModel>, ObservableCollection<FreeSetBindableModel>>(seat.Sets);

                var newSeat = new SeatWithFreeSetsBindableModel
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    Sets = freeSets,
                };

                Order.Seats.Add(newSeat);
            }
        }

        // temporary unused because the products of set are not saved
        //private void SetProductsNamesForSets(ObservableCollection<SetBindableModel> setBindables, ObservableCollection<FreeSetBindableModel> freeSets)
        //{
        //    for (int i = 0; i < setBindables.Count; i++)
        //    {
        //        freeSets[i].ProductNames = string.Join(", ", setBindables[i].Products.Select(x => x.Title));
        //    }
        //}
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

        private async void UpdateCustomerAsync(CustomerModel customer)
        {
            if (Order.IsOrderEditing)
            {
                _orderService.CurrentOrder.Customer = customer;
            }
            else
            {
                var getOrderResult = await _orderService.GetOrderByIdAsync(Order.Id);

                if (getOrderResult.IsSuccess)
                {
                    var order = getOrderResult.Result;
                    order.Customer = customer;

                    var updateOrderResult = await _orderService.UpdateOrderAsync(order);
                }
            }
        }

        private async void AddNewCustomerDialogCallBackAsync(IDialogParameters parameters)
        {
            await _popupNavigation.PopAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.ID, out int newCustomerId))
            {
                var customerResult = await _customersService.GetAllCustomersAsync(x => x.Id == newCustomerId);

                if (customerResult.IsSuccess)
                {
                    var newCustomer = customerResult.Result.FirstOrDefault();

                    UpdateCustomerAsync(newCustomer);

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
                    ApplyCancelRewardToSet(Order.Seats, selectedReward);
                    LockUnlockSimilarRewards(Order.Seats, selectedReward);
                }
                else
                {
                    ObservableCollection<SeatWithFreeSetsBindableModel> setsForRewardApplying = new (Order.Seats);

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

        private Task OnGoToCompleteTabCommandAsync()
        {
            GoToPaymentStep(EPaymentPageSteps.Complete);

            return Task.CompletedTask;
        }

        #endregion
    }
}
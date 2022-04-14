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

        private int _paidOrderId;

        public RewardsViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            IMapper mapper,
            IOrderService orderService,
            ICustomersService customersService,
            IRewardsService rewardService,
            Action<NavigationMessage> navigateAsync,
            Action<EPaymentPageSteps> goToPaymentStep)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;
            _mapper = mapper;
            _orderService = orderService;
            _customersService = customersService;
            _rewardService = rewardService;

            NavigateAsync = navigateAsync;
            GoToPaymentStep = goToPaymentStep;
        }

        #region -- Public properties --

        public ERewardsPageState PageState { get; set; }

        public CustomerModel Customer { get; set; } = new ();

        public bool IsAnyRewardsApplied { get; set; }

        public ObservableCollection<RewardBindabledModel> Rewards { get; set; } = new ();

        public ObservableCollection<SeatWithFreeSetsBindableModel> Seats { get; set; } = new ();

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

            if (parameters.TryGetValue(Constants.Navigations.ORDER_ID, out int paidOrderId))
            {
                _paidOrderId = paidOrderId;
            }

            if (!App.IsTablet && parameters.TryGetValue(Constants.Navigations.IS_REWARD_APPLIED, out bool isRewardApplied)
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
                await RefreshPageDataAsync();
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task RefreshPageDataAsync()
        {
            bool isPaymentForUnsavedOrder = _paidOrderId == 0;

            if (isPaymentForUnsavedOrder)
            {
                Customer = _orderService.CurrentOrder.Customer;
            }
            else
            {
                var orderResult = await _orderService.GetOrderByIdAsync(_paidOrderId);

                if (orderResult.IsSuccess && orderResult.Result.Customer.Id != 0)
                {
                    Customer = orderResult.Result.Customer;
                }
            }

            if (Customer.Id == 0)
            {
                PageState = ERewardsPageState.NoSelectedCustomer;
            }
            else
            {
                var customersRewardsResult = await _rewardService.GetCustomersRewards(Customer.Id);

                if (!customersRewardsResult.IsSuccess)
                {
                    PageState = ERewardsPageState.RewardsNotExist;
                }
                else
                {
                    PageState = ERewardsPageState.RewardsExist;

                    await LoadSeats();

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
        }

        public async Task LoadSeats()
        {
            IEnumerable<SeatModel> seats = new List<SeatModel>();

            bool isPaymentOfUnsavedOrder = _paidOrderId == 0;

            if (isPaymentOfUnsavedOrder)
            {
                var bindableSeats = _orderService.CurrentOrder.Seats.Where(x => x.Sets.Any());

                seats = _mapper.Map<IEnumerable<SeatModel>>(bindableSeats);
            }
            else
            {
                var seatsResult = await _orderService.GetSeatsAsync(_paidOrderId);

                if (seatsResult.IsSuccess)
                {
                    seats = seatsResult.Result;
                }
            }

            Seats.Clear();

            foreach (var seat in seats)
            {
                var freeSets = _mapper.Map<List<SetModel>, ObservableCollection<FreeSetBindableModel>>(seat.Sets);

                var newSeat = new SeatWithFreeSetsBindableModel
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    Sets = freeSets,
                };

                Seats.Add(newSeat);
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

        private Task OnAddNewCustomerCommandAsync()
        {
            var param = new DialogParameters();

            PopupPage popupPage = App.IsTablet
                ? new Views.Tablet.Dialogs.CustomerAddDialog(param, AddNewCustomerDialogCallBack, _customersService)
                : new Views.Mobile.Dialogs.CustomerAddDialog(param, AddNewCustomerDialogCallBack, _customersService);

            return _popupNavigation.PushAsync(popupPage);
        }

        private async void AddNewCustomerDialogCallBack(IDialogParameters parameters)
        {
            await _popupNavigation.PopAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.ID, out int newCustomerId))
            {
                var customerResult = await _customersService.GetAllCustomersAsync(x => x.Id == newCustomerId);

                if (customerResult.IsSuccess)
                {
                    var customer = customerResult.Result.FirstOrDefault();

                    if (_paidOrderId == 0)
                    {
                        _orderService.CurrentOrder.Customer = customer;
                    }
                    else
                    {
                        var getOrderResult = await _orderService.GetOrderByIdAsync(_paidOrderId);

                        if (getOrderResult.IsSuccess)
                        {
                            getOrderResult.Result.Customer = customer;

                            var orderUpdateResult = await _orderService.UpdateOrderAsync(order);
                        }
                    }

                    await RefreshPageDataAsync();
                }
            }
        }

        private Task OnGoToCompleteTabCommandAsync()
        {
            GoToPaymentStep(EPaymentPageSteps.Complete);

            return Task.CompletedTask;
        }

        #endregion
    }
}
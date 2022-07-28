using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Customers;
using Next2.Services.Order;
using Next2.Services.Rewards;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
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
            PaidOrderBindableModel order,
            Action<NavigationMessage> navigateAsync,
            Action<EPaymentSteps> goToPaymentStep)
            : base(navigationService)
        {
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

                    if (isRewardApplied && parameters.TryGetValue(Constants.Navigations.SEATS, out ObservableCollection<SeatWithFreeDishesBindableModel> seats))
                    {
                        Order.Seats = seats;
                        LockUnlockSimilarRewards(Order.Seats, reward);
                    }
                    else
                    {
                        ApplyCancelRewardToDish(Order.Seats, reward);
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
            Order.Customer = _orderService.CurrentOrder.Customer;

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
                            reward.CanBeApplied = Order.Seats.Any(x => x.Dishes.Any(x => x.Id == reward.DishId));
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
            //    var freeDishes = _mapper.Map<ObservableCollection<FreeDishBindableModel>>(seat.SelectedDishes);

            //    if (App.IsTablet)
            //    {
            //        SetProductsNamesForDishes(seat.SelectedDishes, freeDishes);
            //    }

            //    var newSeat = new SeatWithFreeDishesBindableModel
            //    {
            //        Id = seat.Id,
            //        SeatNumber = seat.SeatNumber,
            //        Dishes = freeDishes,
            //    };

            //    Order.Seats.Add(newSeat);
            //}
        }

        //private void SetProductsNamesForDishes(ObservableCollection<DishBindableModel> dishBindables, ObservableCollection<FreeDishBindableModel> freeDishes)
        //{
        //    //for (int i = 0; i < dishBindables.Count; i++)
        //    //{
        //    //    freeDishes[i].ProductNames = string.Join(", ", dishBindables[i].Products.Select(x => x.Title));
        //    //}
        //}
        private void ApplyCancelRewardToDish(ObservableCollection<SeatWithFreeDishesBindableModel> seats, RewardBindabledModel reward)
        {
            bool CompareDishWithRewardDish(FreeDishBindableModel dish) => dish.Id == reward.DishId && dish.IsFree != reward.IsApplied;

            var seat = reward.IsApplied
                ? seats.FirstOrDefault(x => x.Dishes.Any(CompareDishWithRewardDish))
                : seats.LastOrDefault(x => x.Dishes.Any(CompareDishWithRewardDish));

            if (seat is not null)
            {
                var dish = reward.IsApplied
                     ? seat.Dishes.FirstOrDefault(CompareDishWithRewardDish)
                     : seat.Dishes.LastOrDefault(CompareDishWithRewardDish);

                dish.IsFree = reward.IsApplied;
                seat.Dishes = new (seat.Dishes);
            }

            Order.IsUnsavedChangesExist = Order.Rewards.Any(x => x.IsApplied);
        }

        public void LockUnlockSimilarRewards(ObservableCollection<SeatWithFreeDishesBindableModel> seats, RewardBindabledModel selectedReward)
        {
            foreach (var reward in Order.Rewards)
            {
                if (!reward.IsApplied && reward.DishId == selectedReward.DishId)
                {
                    reward.CanBeApplied = seats.Any(seat => seat.Dishes.Any(dish => dish.Id == selectedReward.DishId && !dish.IsFree));
                }
            }
        }

        private async void AddNewCustomerDialogCallBackAsync(IDialogParameters parameters)
        {
            await CloseAllPopupAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.CUSTOMER, out CustomerBindableModel customer))
            {
                var resultOfCreatingNewCustomer = await _customersService.CreateCustomerAsync(customer);

                if (resultOfCreatingNewCustomer.IsSuccess)
                {
                    var resultOfGettingCustomer = await _customersService.GetCustomersAsync(x => x.Id == resultOfCreatingNewCustomer.Result);

                    if (resultOfGettingCustomer.IsSuccess)
                    {
                        _orderService.CurrentOrder.Customer = resultOfGettingCustomer.Result.FirstOrDefault();

                        await RefreshPageDataAsync();
                    }
                    else
                    {
                        await ResponseToBadRequestAsync(resultOfGettingCustomer.Exception?.Message);
                    }
                }
                else
                {
                    await ResponseToBadRequestAsync(resultOfCreatingNewCustomer.Exception?.Message);
                }
            }
        }

        private Task OnAddNewCustomerCommandAsync()
        {
            var param = new DialogParameters();

            PopupPage popupPage = App.IsTablet
                ? new Views.Tablet.Dialogs.CustomerAddDialog(param, AddNewCustomerDialogCallBackAsync)
                : new Views.Mobile.Dialogs.CustomerAddDialog(param, AddNewCustomerDialogCallBackAsync);

            return PopupNavigation.PushAsync(popupPage);
        }

        private Task OnSelectRewardCommandAsync(RewardBindabledModel? selectedReward)
        {
            if (selectedReward is not null)
            {
                if (App.IsTablet)
                {
                    selectedReward.IsApplied = !selectedReward.IsApplied;
                    ApplyCancelRewardToDish(Order.Seats, selectedReward);
                    LockUnlockSimilarRewards(Order.Seats, selectedReward);
                }
                else if (selectedReward.IsApplied)
                {
                    selectedReward.IsApplied = false;
                    selectedReward.IsConfirmedApply = false;
                    ApplyCancelRewardToDish(Order.Seats, selectedReward);
                    LockUnlockSimilarRewards(Order.Seats, selectedReward);
                }
                else
                {
                    ObservableCollection<SeatWithFreeDishesBindableModel> seatsForRewardApplying = new (Order.Seats);

                    selectedReward.IsApplied = true;
                    ApplyCancelRewardToDish(seatsForRewardApplying, selectedReward);

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
using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.CustomersService;
using Next2.Services.Order;
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

        public CustomerRewardsViewModel(
            INavigationService navigationService,
            IMapper mapper,
            IOrderService orderService,
            ICustomersService customersService)
            : base(navigationService)
        {
            _mapper = mapper;
            _orderService = orderService;
            _customersService = customersService;

            _tapPaymentOptionCommand = new AsyncCommand<PaymentItem>(OnTapPaymentOptionCommandAsync, allowsMultipleExecutions: false);

            PaymentOptionsItems = new()
            {
                new()
                {
                    PayemenType = EPaymentItems.Tips,
                    Text = "Tips",
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PayemenType = EPaymentItems.GiftCards,
                    Text = "Gift Cards",
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PayemenType = EPaymentItems.Cash,
                    Text = "Cash",
                    TapCommand = _tapPaymentOptionCommand,
                },
                new()
                {
                    PayemenType = EPaymentItems.Card,
                    Text = "Card",
                    TapCommand = _tapPaymentOptionCommand,
                },
            };

            SelectedPaymentOption = PaymentOptionsItems[3];
            CardPaymentStatus = ECardPaymentStatus.WaitingSwipeCard;
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

        private ICommand _changeCardPaymentStatusCommand;
        public ICommand ChangeCardPaymentStatusCommand => _changeCardPaymentStatusCommand ??= new AsyncCommand(OnChangeCardPaymentStatusCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearDrawPanelCommand;
        public ICommand ClearDrawPanelCommand => _clearDrawPanelCommand ??= new AsyncCommand(OnClearDrawPanelCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapPaymentOptionCommand;

        public ECardPaymentStatus CardPaymentStatus { get; set; }

        public bool IsCleared { get; set; } = true;

        public bool IsExpandedSummary { get; set; } = true;

        public ObservableCollection<PaymentItem> PaymentOptionsItems { get; set; } = new();

        public PaymentItem SelectedPaymentOption { get; set; } = new();

        private async Task OnSelectRewardCommandAsync(RewardBindabledModel selectedReward)
        {
            if (selectedReward is not null)
            {
                if (App.IsTablet)
                {
                    ApplyCancelReward(selectedReward);
                }
                else
                {
                    var parameters = new NavigationParameters
                    {
                        { Constants.Navigations.SEATS, Seats },
                    };

                    await _navigationService.NavigateAsync(nameof(OrderWithRewardsPage), parameters);
                }
            }
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            var customer = _orderService.CurrentOrder.Customer;

            await LoadPageData(customer);
        }

        #endregion

        #region -- Private helpers --

        private async Task OnClearDrawPanelCommandAsync()
        {
            IsCleared = true;
        }

        private async Task OnChangeCardPaymentStatusCommandAsync()
        {
            CardPaymentStatus = ECardPaymentStatus.WaitingSignature;
        }

        private async Task OnTapPaymentOptionCommandAsync(PaymentItem item)
        {
            switch (item.PayemenType)
            {
                case EPaymentItems.Cash:
                    _navigationService.NavigateAsync(nameof(InputCashPage));
                    break;
            }
        }

        private async Task LoadPageData(CustomerModel customer)
        {
            if (customer.Name == string.Empty)
            {
                PageState = ECustomerRewardsPageState.NoSelectedCustomer;
            }
            else
            {
                CustomerName = customer.Name;

                var customersRewardsResult = await _orderService.GetCustomersRewards(customer.Id);

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

        private void ApplyCancelReward(RewardBindabledModel selectedReward)
        {
            selectedReward.IsApplied = !selectedReward.IsApplied;

            IsAnyRewardsApplied = Rewards.Any(x => x.IsApplied);

            Func<FreeSetBindableModel, bool> setComparer = y => y.Id == selectedReward.SetId && y.IsFree != selectedReward.IsApplied;

            var seat = selectedReward.IsApplied
                ? Seats.FirstOrDefault(x => x.Sets.Any(setComparer))
                : Seats.LastOrDefault(x => x.Sets.Any(setComparer));

            if (seat is not null)
            {
                var set = selectedReward.IsApplied
                     ? seat.Sets.FirstOrDefault(setComparer)
                     : seat.Sets.LastOrDefault(setComparer);

                set.IsFree = selectedReward.IsApplied;
                seat.Sets = new(seat.Sets);

                LockUnlockSimilarRewards(selectedReward);
            }
        }

        public void LockUnlockSimilarRewards(RewardBindabledModel selectedRewardSet)
        {
            foreach (var reward in Rewards)
            {
                if (!reward.IsApplied && reward.SetId == selectedRewardSet.SetId)
                {
                    reward.IsCanBeApplied = Seats.Any(seat => seat.Sets.Any(set => set.Id == selectedRewardSet.SetId && !set.IsFree));
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
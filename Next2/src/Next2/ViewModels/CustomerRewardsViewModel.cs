using AutoMapper;
using Next2.Enums;
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
        }

        #region -- Public properties --

        public bool IsAnyRewardsSelected { get; set; }

        public ObservableCollection<RewardBindabledModel> Rewards { get; set; } = new ();

        public ObservableCollection<SeatWithDiscountedBindableModel> Seats { get; set; } = new ();

        public ECustomerRewardsPageState PageState { get; set; }

        private ICommand _addNewCustomerCommand;
        public ICommand AddNewCustomerCommand => _addNewCustomerCommand ??= new AsyncCommand(OnAddNewCustomerCommandAsync, allowsMultipleExecutions: false);

        private ICommand _selectRewardCommand;
        public ICommand SelectRewardCommand => _selectRewardCommand ??= new AsyncCommand<object>(OnSeletRewardCommandAsync, allowsMultipleExecutions: false);

        private async Task OnSeletRewardCommandAsync(object rewardObj)
        {
            if (rewardObj is RewardBindabledModel reward)
            {
                if (App.IsTablet)
                {
                    IsAnyRewardsSelected = Rewards.Any(x => x.IsSelected);

                    reward.IsSelected = !reward.IsSelected;

                    Func<DiscountedSetBindableModel, bool> rewardSetComparer = y => y.IsFree != reward.IsSelected && y.Id == reward.SetId;

                    var seat = reward.IsSelected
                        ? Seats.FirstOrDefault(x => x.Sets.Any(rewardSetComparer))
                        : Seats.LastOrDefault(x => x.Sets.Any(rewardSetComparer));

                    if (seat is not null)
                    {
                        var set = reward.IsSelected
                             ? seat.Sets.FirstOrDefault(rewardSetComparer)
                             : seat.Sets.LastOrDefault(rewardSetComparer);

                        set.IsFree = reward.IsSelected;
                        seat.Sets = new (seat.Sets);
                    }
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

        private async Task LoadPageData(CustomerModel customer)
        {
            if (customer.Name == string.Empty)
            {
                PageState = ECustomerRewardsPageState.NoSelectedCustomer;
            }
            else
            {
                var customersRewardsResult = await _orderService.GetCustomersRewards(customer.Id);

                if (!customersRewardsResult.IsSuccess)
                {
                    PageState = ECustomerRewardsPageState.RewardsNotExist;
                }
                else
                {
                    PageState = ECustomerRewardsPageState.RewardsExist;

                    Rewards = _mapper.Map<IEnumerable<RewardModel>, ObservableCollection<RewardBindabledModel>>(customersRewardsResult.Result);

                    foreach (var item in Rewards)
                    {
                        item.SelectCommand = SelectRewardCommand;
                    }

                    await LoadSeats();
                }
            }
        }

        public Task LoadSeats()
        {
            var seats = _orderService.CurrentOrder.Seats.Where(x => x.Sets.Any());

            foreach (var seat in seats)
            {
                var disocuntedSets = _mapper.Map<ObservableCollection<SetBindableModel>, ObservableCollection<DiscountedSetBindableModel>>(seat.Sets);

                var newSeat = new SeatWithDiscountedBindableModel
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    Sets = disocuntedSets,
                };

                Seats.Add(newSeat);
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

                await LoadPageData(customer);
            }
        }

        #endregion
    }
}
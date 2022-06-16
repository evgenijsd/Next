using AutoMapper;
using Next2.Enums;
using Next2.Helpers.Events;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Bonuses;
using Next2.Services.Order;
using Prism.Events;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class BonusPageViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly IEventAggregator _eventAggregator;
        private readonly IBonusesService _bonusesService;
        private readonly IOrderService _orderService;
        private readonly double _heightBonus = App.IsTablet ? Constants.LayoutBonuses.ROW_TABLET_BONUS : Constants.LayoutBonuses.ROW_MOBILE_BONUS;

        private List<BonusModel> _bonuses = new();

        public BonusPageViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IOrderService orderService,
            IMapper mapper,
            IBonusesService bonusesService)
            : base(navigationService)
        {
            _mapper = mapper;
            _bonusesService = bonusesService;
            _eventAggregator = eventAggregator;
            _orderService = orderService;
        }

        #region -- Public properties --
        public ObservableCollection<BonusBindableModel> Coupons { get; set; } = new();

        public ObservableCollection<BonusBindableModel> Discounts { get; set; } = new();

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public BonusBindableModel? SelectedBonus { get; set; }

        public string Title { get; set; } = string.Empty;

        public double HeightCoupons { get; set; } = 0;

        public double HeightDiscounts { get; set; } = 0;

        private ICommand _ApplyBonusCommand;
        public ICommand ApplyBonusCommand => _ApplyBonusCommand ??= new AsyncCommand(OnApplyBonusCommandAsync, allowsMultipleExecutions: false);

        private ICommand _RemoveSelectionBonusCommand;
        public ICommand RemoveSelectionBonusCommand => _RemoveSelectionBonusCommand ??= new AsyncCommand(OnRemoveSelectionBonusCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapSelectBonusCommand;
        public ICommand TapSelectBonusCommand => _tapSelectBonusCommand ??= new AsyncCommand<BonusBindableModel?>(OnTapSelectBonusCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapSelectCollapceCommand;
        public ICommand TapSelectCollapceCommand => _tapSelectCollapceCommand ??= new AsyncCommand<EBonusType>(OnTapSelectCollapceCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.CURRENT_ORDER, out FullOrderBindableModel currentOrder))
            {
                CurrentOrder = _mapper.Map<FullOrderBindableModel>(currentOrder);

                var seats = new ObservableCollection<SeatBindableModel>();

                foreach (var seat in CurrentOrder.Seats)
                {
                    var selectedDishes = _mapper.Map<ObservableCollection<DishBindableModel>>(seat.SelectedDishes);
                    var newSeat = _mapper.Map<SeatBindableModel>(seat);
                    newSeat.SelectedDishes = selectedDishes;
                    seats.Add(newSeat);
                }

                CurrentOrder.Seats = seats;

                var coupons = await GetCoupons();

                if (coupons is not null)
                {
                    Coupons = _mapper.Map<IEnumerable<CouponModelDTO>, ObservableCollection<BonusBindableModel>>(coupons);

                    foreach (var coupon in Coupons)
                    {
                        coupon.TapCommand = TapSelectBonusCommand;
                        coupon.Type = EBonusType.Coupone;
                    }
                }

                var discounts = await GetDiscounts();

                if (discounts is not null)
                {
                    Discounts = _mapper.Map<IEnumerable<DiscountModelDTO>, ObservableCollection<BonusBindableModel>>(discounts);

                    foreach (var discount in Discounts)
                    {
                        discount.TapCommand = TapSelectBonusCommand;
                        discount.Type = EBonusType.Discount;
                    }
                }

                HeightCoupons = Coupons.Count * _heightBonus;
                HeightDiscounts = Discounts.Count * _heightBonus;

                if (CurrentOrder.Coupon is not null)
                {
                    var couponId = CurrentOrder.Coupon.Id;
                    SelectedBonus = Coupons.FirstOrDefault(row => row.Id == couponId);
                }
                else if(CurrentOrder.Discount is not null)
                {
                    var discountId = CurrentOrder.Discount.Id;
                    SelectedBonus = Discounts.FirstOrDefault(row => row.Id == discountId);
                }
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedBonus))
            {
                Title = SelectedBonus is null
                    ? string.Empty
                    : SelectedBonus.Name;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task<IEnumerable<CouponModelDTO>>? GetCoupons()
        {
            IEnumerable<CouponModelDTO>? result = null;
            var dishesIds = CurrentOrder.Seats.SelectMany(x => x.SelectedDishes).Select(x => x.Id);

            var response = await _bonusesService.GetCouponsAsync(x => x.IsActive && x.Dishes.Select(x => x.Id).Intersect(dishesIds).Any());

            if (response.IsSuccess)
            {
                result = response.Result;
            }

            return result;
        }

        private async Task<IEnumerable<DiscountModelDTO>> GetDiscounts()
        {
            IEnumerable<DiscountModelDTO>? result = null;

            var response = await _bonusesService.GetDiscountsAsync(x => x.IsActive);

            if (response.IsSuccess)
            {
                result = response.Result;
            }

            return result;
        }

        private async Task OnApplyBonusCommandAsync()
        {
            _eventAggregator.GetEvent<AddBonusToCurrentOrderEvent>().Publish(CurrentOrder);

            await _navigationService.GoBackAsync();
        }

        private async Task OnTapSelectBonusCommandAsync(BonusBindableModel? bonus)
        {
            SelectedBonus = bonus == SelectedBonus ? null : bonus;

            if (bonus is not null)
            {
                if (bonus.Type is EBonusType.Coupone)
                {
                    var coupon = _mapper.Map<CouponModelDTO>(bonus);
                    coupon.SeatNumbers = CurrentOrder.Seats.Count;

                    CurrentOrder.Coupon = coupon;
                    CurrentOrder.Discount = null;
                }
                else if (bonus.Type is EBonusType.Discount)
                {
                    CurrentOrder.Discount = _mapper.Map<DiscountModelDTO>(bonus);
                    CurrentOrder.Coupon = null;
                }
            }

            await _bonusesService.СalculationBonusAsync(CurrentOrder);
        }

        private Task OnTapSelectCollapceCommandAsync(EBonusType bonusType)
        {
            if (bonusType == EBonusType.Coupone)
            {
                HeightCoupons = HeightCoupons == 0
                    ? Coupons.Count * _heightBonus
                    : 0;
            }
            else
            {
                HeightDiscounts = HeightDiscounts == 0
                    ? Discounts.Count * _heightBonus
                    : 0;
            }

            return Task.CompletedTask;
        }

        private Task OnRemoveSelectionBonusCommandAsync()
        {
            SelectedBonus = null;
            return Task.CompletedTask;
        }

        #endregion
    }
}
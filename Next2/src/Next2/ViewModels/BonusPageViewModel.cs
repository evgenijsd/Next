using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Authentication;
using Next2.Services.Bonuses;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class BonusPageViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;
        private readonly IBonusesService _bonusesService;

        private int _indexOfSeat;
        private int _indexOfSelectedDish = -1;

        public BonusPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            IOrderService orderService,
            IMapper mapper,
            IBonusesService bonusesService)
            : base(navigationService, authenticationService, notificationsService)
        {
            _mapper = mapper;
            _bonusesService = bonusesService;
            _orderService = orderService;
        }

        #region -- Public properties --

        public ObservableCollection<BonusBindableModel> Coupons { get; set; } = new();

        public ObservableCollection<BonusBindableModel> Discounts { get; set; } = new();

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public BonusBindableModel? SelectedBonus { get; set; }

        public string Title { get; set; } = string.Empty;

        public double HeightCoupons { get; set; } = 0;

        public double HeightDiscounts { get; set; } = 0;

        public bool IsCouponsVisible { get; set; } = true;

        public bool IsDiscountsVisible { get; set; } = true;

        private ICommand? _applyBonusCommand;
        public ICommand ApplyBonusCommand => _applyBonusCommand ??= new AsyncCommand(OnApplyBonusCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _removeSelectionBonusCommand;
        public ICommand RemoveSelectionBonusCommand => _removeSelectionBonusCommand ??= new AsyncCommand(OnRemoveSelectionBonusCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _tapSelectBonusCommand;
        public ICommand TapSelectBonusCommand => _tapSelectBonusCommand ??= new AsyncCommand<BonusBindableModel?>(OnTapSelectBonusCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _tapSelectCollapceCommand;
        public ICommand TapSelectCollapceCommand => _tapSelectCollapceCommand ??= new AsyncCommand<EBonusType>(OnTapSelectCollapceCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.CURRENT_ORDER, out FullOrderBindableModel currentOrder))
            {
                if (IsInternetConnected)
                {
                    var seat = currentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

                    if (seat is not null)
                    {
                        _indexOfSeat = currentOrder.Seats.IndexOf(seat);
                        _indexOfSelectedDish = seat.SelectedDishes.IndexOf(seat.SelectedItem);
                    }

                    CurrentOrder = _mapper.Map<FullOrderBindableModel>(currentOrder);

                    Seats = new(CurrentOrder.Seats.Where(x => x.SelectedDishes.Count > 0));

                    var coupons = await GetCouponsAsync();

                    if (coupons is not null)
                    {
                        Coupons = _mapper.Map<IEnumerable<CouponModelDTO>, ObservableCollection<BonusBindableModel>>(coupons);

                        foreach (var coupon in Coupons)
                        {
                            coupon.TapCommand = TapSelectBonusCommand;
                            coupon.Type = EBonusType.Coupon;
                        }
                    }

                    var discounts = await GetDiscountsAsync();

                    if (discounts is not null)
                    {
                        Discounts = _mapper.Map<IEnumerable<DiscountModelDTO>, ObservableCollection<BonusBindableModel>>(discounts);

                        foreach (var discount in Discounts)
                        {
                            discount.TapCommand = TapSelectBonusCommand;
                            discount.Type = EBonusType.Discount;
                        }
                    }

                    if (CurrentOrder.Coupon is not null)
                    {
                        var couponId = CurrentOrder.Coupon.Id;
                        SelectedBonus = Coupons.FirstOrDefault(row => row.Id == couponId);
                    }
                    else if (CurrentOrder.Discount is not null)
                    {
                        var discountId = CurrentOrder.Discount.Id;
                        SelectedBonus = Discounts.FirstOrDefault(row => row.Id == discountId);
                    }
                }
                else
                {
                    await _notificationsService.ShowNoInternetConnectionDialogAsync();
                }
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedBonus))
            {
                Title = SelectedBonus?.Name ?? string.Empty;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task<IEnumerable<CouponModelDTO>?> GetCouponsAsync()
        {
            IEnumerable<CouponModelDTO>? coupons = null;
            var dishes = CurrentOrder.Seats.SelectMany(x => x.SelectedDishes);

            if (dishes.Any())
            {
                var dishesIds = dishes.Select(x => x.DishId);

                var couponResult = await _bonusesService.GetCouponsAsync(x => x.IsActive && x.Dishes.Select(x => x.Id).Intersect(dishesIds).Any());

                if (couponResult.IsSuccess)
                {
                    coupons = couponResult.Result;
                }
                else
                {
                    await ResponseToUnsuccessfulRequestAsync(couponResult.Exception?.Message);
                }
            }

            return coupons;
        }

        private async Task<IEnumerable<DiscountModelDTO>?> GetDiscountsAsync()
        {
            IEnumerable<DiscountModelDTO>? discounts = null;
            var dishes = CurrentOrder.Seats.SelectMany(x => x.SelectedDishes);

            if (dishes.Any())
            {
                var discountResult = await _bonusesService.GetDiscountsAsync(x => x.IsActive);

                if (discountResult.IsSuccess)
                {
                    discounts = discountResult.Result;
                }
                else
                {
                    await ResponseToUnsuccessfulRequestAsync(discountResult.Exception?.Message);
                }
            }

            return discounts;
        }

        private Task OnApplyBonusCommandAsync()
        {
            if (_indexOfSelectedDish > -1)
            {
                CurrentOrder.Seats[_indexOfSeat].SelectedItem = CurrentOrder.Seats[_indexOfSeat].SelectedDishes[_indexOfSelectedDish];
            }

            var parameters = new NavigationParameters
            {
                { Constants.Navigations.BONUS, CurrentOrder },
            };

            return _navigationService.GoBackAsync(parameters);
        }

        private Task OnTapSelectBonusCommandAsync(BonusBindableModel? bonus)
        {
            SelectedBonus = bonus == SelectedBonus
                ? null
                : bonus;

            if (bonus?.Type is EBonusType.Coupon)
            {
                var coupon = _mapper.Map<CouponModelDTO>(bonus);
                coupon.SeatNumber = CurrentOrder.Seats.Count;

                CurrentOrder.Discount = null;
                CurrentOrder.Coupon = coupon;
            }
            else if (bonus?.Type is EBonusType.Discount)
            {
                CurrentOrder.Coupon = null;
                CurrentOrder.Discount = _mapper.Map<DiscountModelDTO>(bonus);
            }

            _orderService.UpdateTotalSum(CurrentOrder);

            Seats = new(CurrentOrder.Seats.Where(x => x.SelectedDishes.Count > 0));

            return Task.CompletedTask;
        }

        private Task OnTapSelectCollapceCommandAsync(EBonusType bonusType)
        {
            if (bonusType == EBonusType.Coupon)
            {
                IsCouponsVisible = !IsCouponsVisible;
            }
            else
            {
                IsDiscountsVisible = !IsDiscountsVisible;
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
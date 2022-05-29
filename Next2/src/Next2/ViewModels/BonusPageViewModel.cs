using AutoMapper;
using Next2.Enums;
using Next2.Helpers.Events;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Services.Bonuses;
using Next2.Services.Order;
using Prism.Events;
using Prism.Navigation;
using System;
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
                CurrentOrder = currentOrder;

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
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedBonus))
            {
                Title = SelectedBonus is null ? string.Empty : SelectedBonus.Name;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task<IEnumerable<CouponModelDTO>>? GetCoupons()
        {
            IEnumerable<CouponModelDTO>? result = null;
            var couponsIds = CurrentOrder.Seats.SelectMany(x => x.Sets).SelectMany(x => x.Coupons).Select(x => x.Id).Distinct();

            var aoResult = await _bonusesService.GetAllBonusesAsync<CouponModelDTO>(x => x.IsActive && couponsIds.Contains(x.Id));

            if (aoResult.IsSuccess)
            {
                result = aoResult.Result;
            }

            return result;
        }

        private async Task<IEnumerable<DiscountModelDTO>> GetDiscounts()
        {
            IEnumerable<DiscountModelDTO>? result = null;

            var aoResult = await _bonusesService.GetAllBonusesAsync<DiscountModelDTO>(x => x.IsActive);

            if (aoResult.IsSuccess)
            {
                result = aoResult.Result;
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

            if (SelectedBonus is not null)
            {
                if (bonus.Type is EBonusType.Coupone)
                {
                    var coupon = _mapper.Map<CouponModelDTO>(bonus);
                    coupon.SeatNumbers = CurrentOrder.Seats.Count;
                    CurrentOrder.Coupon = coupon;
                }

                if (bonus.Type is EBonusType.Discount)
                {
                    CurrentOrder.Discount = _mapper.Map<DiscountModelDTO>(bonus);
                }

                CurrentOrder = await _bonusesService.СalculationBonusAsync(CurrentOrder);
            }
        }

        private Task OnTapSelectCollapceCommandAsync(EBonusType bonusType)
        {
            if (bonusType == EBonusType.Coupone)
            {
                HeightCoupons = HeightCoupons == 0 ? Coupons.Count * _heightBonus : 0;
            }
            else
            {
                HeightDiscounts = HeightDiscounts == 0 ? Discounts.Count * _heightBonus : 0;
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
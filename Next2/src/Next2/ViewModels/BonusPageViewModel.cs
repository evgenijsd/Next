using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Services.Bonuses;
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
        private readonly IBonusesService _bonusesService;

        private IEnumerable<BonusModel>? _bonuses;

        private IEnumerable<BonusConditionModel>? _bonusConditions;

        private IEnumerable<BonusSetModel>? _bonusSets;

        private IEnumerable<SetModel>? _sets;

        public BonusPageViewModel(
            INavigationService navigationService,
            IBonusesService bonusesService)
            : base(navigationService)
        {
            _bonusesService = bonusesService;
        }

        #region -- Public properties --
        public ObservableCollection<BonusBindableModel> Coupons { get; set; } = new();

        public ObservableCollection<BonusBindableModel> Discounts { get; set; } = new();

        public FullOrderBindableModel CurrentOrder { get; set; } = new();

        public ObservableCollection<SetBindableModel> Sets { get; set; } = new();

        public BonusBindableModel? SelectedCoupon { get; set; }

        public BonusBindableModel? SelectedDiscount { get; set; }

        public string Title { get; set; } = string.Empty;

        public double HeightCoupons { get; set; } = 0;

        public double HeightDiscounts { get; set; } = 0;

        private ICommand _BonusCommand;
        public ICommand BonusCommand => _BonusCommand ??= new AsyncCommand(OnBonusCommandAsync);

        private ICommand _tapSelectCouponCommand;
        public ICommand TapSelectCouponCommand => _tapSelectCouponCommand ??= new AsyncCommand<BonusBindableModel?>(OnTapSelectCouponCommandAsync);

        private ICommand _tapSelectDiscountCommand;
        public ICommand TapSelectDiscountCommand => _tapSelectDiscountCommand ??= new AsyncCommand<BonusBindableModel?>(OnTapSelectDiscountCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            var result = await _bonusesService.GetBonusesAsync();

            if (result.IsSuccess)
            {
                _bonuses = result.Result;
                var config = new MapperConfiguration(cfg => cfg.CreateMap<BonusModel, BonusBindableModel>());
                var mapper = new Mapper(config);

                var resultConditions = await _bonusesService.GetConditionsAsync();

                if (resultConditions.IsSuccess)
                {
                    _bonusConditions = resultConditions.Result;
                    Discounts = mapper.Map<IEnumerable<BonusModel>, ObservableCollection<BonusBindableModel>>(_bonuses.Where(x => _bonusConditions.Any(y => y.BonusId == x.Id)));
                    Coupons = mapper.Map<IEnumerable<BonusModel>, ObservableCollection<BonusBindableModel>>(_bonuses.Where(x => !_bonusConditions.Any(y => y.BonusId == x.Id)));
                }

                HeightCoupons = Coupons.Count * Constants.LayoutBonuses.ROW_BONUS;
                HeightDiscounts = Discounts.Count * Constants.LayoutBonuses.ROW_BONUS;
            }

            if (parameters.TryGetValue(Constants.Navigations.CURRENT_ORDER, out FullOrderBindableModel currentOrder))
            {
                CurrentOrder = currentOrder;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedCoupon) || args.PropertyName is nameof(SelectedDiscount))
            {
                Title = SelectedCoupon?.Name + SelectedDiscount?.Name;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnBonusCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        private Task OnTapSelectCouponCommandAsync(BonusBindableModel? coupon)
        {
            SelectedDiscount = null;
            CurrentOrder.BonusType = EBonusType.None;

            SelectedCoupon = coupon == SelectedCoupon ? null : coupon;

            if (SelectedCoupon is not null)
            {
                CurrentOrder.BonusType = EBonusType.Coupone;

                foreach (SeatBindableModel seat in CurrentOrder.Seats)
                {
                    foreach (SetBindableModel set in seat.Sets)
                    {
                        if (SelectedCoupon.Type == EBonusValueType.Percent)
                        {
                            set.PriceBonus = (float)(set.Price - (SelectedCoupon.Value * set.Price));
                        }
                        else
                        {
                            set.PriceBonus = (float)(set.Price - SelectedCoupon.Value);
                        }

                        if (set.PriceBonus < 0)
                        {
                            set.PriceBonus = 0;
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task OnTapSelectDiscountCommandAsync(BonusBindableModel? discount)
        {
            SelectedCoupon = null;
            CurrentOrder.BonusType = EBonusType.None;

            SelectedDiscount = discount == SelectedDiscount ? null : discount;

            if (SelectedDiscount is not null)
            {
                CurrentOrder.BonusType = EBonusType.Discount;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
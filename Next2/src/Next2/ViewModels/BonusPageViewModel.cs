using AutoMapper;
using Next2.Models;
using Next2.Services.Bonuses;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class BonusPageViewModel : BaseViewModel
    {
        private readonly IBonusesService _bonusesService;

        private IEnumerable<BonusModel>? _bonuses;

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

        public BonusBindableModel? SelectedCoupon { get; set; }

        public BonusBindableModel? SelectedDiscount { get; set; }

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

        public override async void OnAppearing()
        {
            base.OnAppearing();

            var result = await _bonusesService.GetBonusesAsync();

            if (result.IsSuccess)
            {
                _bonuses = new List<BonusModel>(result.Result);
                var config = new MapperConfiguration(cfg => cfg.CreateMap<BonusModel, BonusBindableModel>());
                var mapper = new Mapper(config);
                Coupons = mapper.Map<IEnumerable<BonusModel>, ObservableCollection<BonusBindableModel>>(result.Result);
                Discounts = mapper.Map<IEnumerable<BonusModel>, ObservableCollection<BonusBindableModel>>(result.Result);
                HeightCoupons = Coupons.Count * Constants.LayoutBonuses.ROW_BONUS;
                HeightDiscounts = Discounts.Count * Constants.LayoutBonuses.ROW_BONUS;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedCoupon) || args.PropertyName is nameof(SelectedDiscount))
            {
                var i = 0;
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

            SelectedCoupon = coupon == SelectedCoupon ? null : coupon;

            return Task.CompletedTask;
        }

        private Task OnTapSelectDiscountCommandAsync(BonusBindableModel? discount)
        {
            SelectedCoupon = null;

            SelectedDiscount = discount == SelectedDiscount ? null : discount;

            return Task.CompletedTask;
        }

        #endregion
    }
}
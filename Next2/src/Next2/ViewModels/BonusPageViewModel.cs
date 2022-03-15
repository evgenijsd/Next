using AutoMapper;
using Next2.Models;
using Next2.Services.Bonuses;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<BonusBindableModel> Bonuses { get; set; } = new();

        public double HeightCoupons { get; set; } = 0;

        public double HeightDiscounts { get; set; } = 0;

        private ICommand _BonusCommand;
        public ICommand BonusCommand => _BonusCommand ??= new AsyncCommand(OnBonusCommandAsync);

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
                Bonuses = mapper.Map<IEnumerable<BonusModel>, ObservableCollection<BonusBindableModel>>(result.Result);
                HeightCoupons = Bonuses.Count * 60;
                HeightDiscounts = Bonuses.Count * 60;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnBonusCommandAsync()
        {
            await _navigationService.GoBackAsync();
        }

        #endregion
    }
}
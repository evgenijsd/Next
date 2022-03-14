using Next2.Models;
using Next2.Services.Bonuses;
using Prism.Navigation;
using System.Collections.Generic;
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
using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Bonuses;
using Next2.Services.Order;
using Next2.Views.Mobile;
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
        private readonly IEventAggregator _eventAggregator;
        private readonly IBonusesService _bonusesService;
        private readonly IOrderService _orderService;
        private readonly double _heightBonus = App.IsTablet ? Constants.LayoutBonuses.ROW_TABLET_BONUS : Constants.LayoutBonuses.ROW_MOBILE_BONUS;

        private IEnumerable<BonusModel>? _bonuses;
        private IEnumerable<BonusConditionModel>? _bonusConditions;
        public IEnumerable<BonusSetModel>? _bonusSets;

        public BonusPageViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IOrderService orderService,
            IBonusesService bonusesService)
            : base(navigationService)
        {
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

        private ICommand _BonusCommand;
        public ICommand BonusCommand => _BonusCommand ??= new AsyncCommand(OnBonusCommandAsync);

        private ICommand _RemoveSelectionBonusCommand;
        public ICommand RemoveSelectionBonusCommand => _RemoveSelectionBonusCommand ??= new AsyncCommand(OnRemoveSelectionBonusCommandAsync);

        private ICommand _tapSelectBonusCommand;
        public ICommand TapSelectBonusCommand => _tapSelectBonusCommand ??= new AsyncCommand<BonusBindableModel?>(OnTapSelectBonusCommandAsync);

        private ICommand _tapSelectCollapceCommand;
        public ICommand TapSelectCollapceCommand => _tapSelectCollapceCommand ??= new AsyncCommand<EBonusType>(OnTapSelectCollapceCommandAsync);

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

                var resultBonusSets = await _bonusesService.GetBonusSetsAsync();

                if (resultBonusSets.IsSuccess)
                {
                    _bonusSets = resultBonusSets.Result;
                }

                HeightCoupons = Coupons.Count * _heightBonus;
                HeightDiscounts = Discounts.Count * _heightBonus;
            }

            if (parameters.TryGetValue(Constants.Navigations.CURRENT_ORDER, out FullOrderBindableModel currentOrder))
            {
                CurrentOrder = currentOrder;
                var sets = _bonusesService.GetSets(CurrentOrder);

                if (_bonusConditions is not null)
                {
                    Discounts = _bonusesService.GetDiscounts(_bonusConditions, Discounts, sets);
                }
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

        private async Task OnBonusCommandAsync()
        {
            _eventAggregator.GetEvent<BonusEvent>().Publish(CurrentOrder);

            await _navigationService.GoBackAsync();
        }

        private Task OnTapSelectBonusCommandAsync(BonusBindableModel? bonus)
        {
            CurrentOrder.BonusType = EBonusType.None;

            SelectedBonus = bonus == SelectedBonus ? null : bonus;

            if (SelectedBonus is not null)
            {
                CurrentOrder.BonusType = Discounts.Any(x => x.Id == SelectedBonus.Id) ? EBonusType.Discount : EBonusType.Coupone;
                CurrentOrder.Bonus = SelectedBonus;
                CurrentOrder.PriceWithBonus = 0;

                var bonusConditions = _bonusConditions?.Where(x => x.BonusId == SelectedBonus.Id).ToList() ?? Enumerable.Empty<BonusConditionModel>().ToList();
                var bonusSets = _bonusSets?.Where(x => x.BonusId == SelectedBonus.Id).ToList() ?? Enumerable.Empty<BonusSetModel>().ToList();
                List<SetBindableModel> setConditions = new();
                List<SetBindableModel> setBonus = new();
                int countCondition;
                int countBonusSet;
                var sets = _bonusesService.GetSets(CurrentOrder).ToList();

                do
                {
                    countCondition = 0;

                    foreach (var condition in bonusConditions)
                    {
                        var set = sets?.FirstOrDefault(x => x.Id == condition.SetId);

                        if (set is not null)
                        {
                            setConditions.Add(set);
                            sets?.Remove(set);
                            countCondition++;
                        }
                    }

                    countBonusSet = 0;

                    foreach (var bonusSet in bonusSets)
                    {
                        var set = sets?.FirstOrDefault(x => x.Id == bonusSet.SetId);

                        if (set is not null)
                        {
                            setBonus.Add(set);
                            sets?.Remove(set);
                            countBonusSet++;
                        }
                    }
                }
                while ((countCondition == bonusConditions.Count) && (countBonusSet == bonusSets.Count));

                while(countBonusSet > 0 && countBonusSet < bonusSets.Count)
                {
                    setBonus.Remove(setBonus[^1]);
                    countBonusSet--;
                }

                while (countCondition > 0 && countCondition < bonusConditions.Count)
                {
                    setConditions.Remove(setConditions[^1]);
                    countCondition--;
                }

                foreach (SeatBindableModel seat in CurrentOrder.Seats)
                {
                    foreach (SetBindableModel set in seat.Sets)
                    {
                        if (setBonus is null || !setConditions.Any(x => x.Id == set.Id) || setBonus.Any(x => x.Id == set.Id))
                        {
                            set.PriceBonus = _bonusesService.GetPriceBonus(SelectedBonus, set);
                        }
                        else
                        {
                            set.PriceBonus = set.Portion.Price;
                        }

                        CurrentOrder.PriceWithBonus += set.PriceBonus;
                    }

                    CurrentOrder.PriceTax = CurrentOrder.PriceWithBonus * CurrentOrder.Tax.Value;

                    CurrentOrder.Total = CurrentOrder.PriceWithBonus + CurrentOrder.PriceTax;
                }
            }

            return Task.CompletedTask;
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
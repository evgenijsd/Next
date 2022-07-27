using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Models.Bindables;
using Next2.Services.OrdersHolding;
using Next2.Services.Notifications;
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
    public class HoldDishesViewModel : BaseViewModel
    {
        private readonly IOrdersHolding _ordersHolding;
        private readonly INotificationsService _notificationsService;
        private readonly IMapper _mapper;

        private EHoldDishesSortingType _holdDishesSortingType;

        public HoldDishesViewModel(
            IOrdersHolding ordersHolding,
            INotificationsService notificationsService,
            IMapper mapper,
            INavigationService navigationService)
            : base(navigationService)
        {
            _ordersHolding = ordersHolding;
            _notificationsService = notificationsService;
            _mapper = mapper;
        }

        #region -- Public properties --

        public bool IsHoldDishesRefreshing { get; set; }

        public bool IsNothingFound { get; set; }

        public int IndexLastVisibleElement { get; set; }

        public ObservableCollection<HoldDishBindableModel> HoldDishes { get; set; } = new();

        public ObservableCollection<object>? SelectedHoldDishes { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new();

        public TableBindableModel? SelectedTable { get; set; }

        private ICommand _refreshHoldDishesCommand;
        public ICommand RefreshHoldDishesCommand => _refreshHoldDishesCommand ??= new AsyncCommand(OnRefreshHoldDishesCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changeSortHoldDishesCommand;
        public ICommand ChangeSortHoldDishesCommand => _changeSortHoldDishesCommand ??= new AsyncCommand<EHoldDishesSortingType>(OnChangeSortHoldDishesCommandAsync, allowsMultipleExecutions: false);

        private ICommand _setSelectedHoldDishesCommand;
        public ICommand SetSelectedHoldDishesCommand => _setSelectedHoldDishesCommand ??= new AsyncCommand<List<object>?>(OnSetSelectedHoldDishesCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapSelectAllHoldDishesTableCommand;
        public ICommand TapSelectAllHoldDishesTableCommand => _tapSelectAllHoldDishesTableCommand ??= new AsyncCommand(OnTapSelectAllHoldDishesTableCommandAsync, allowsMultipleExecutions: false);

        private ICommand _extendCommand;
        public ICommand ExtendCommand => _extendCommand ??= new AsyncCommand(OnExtendCommandAsync, allowsMultipleExecutions: false);

        private ICommand _releaseCommand;
        public ICommand ReleaseCommand => _releaseCommand ??= new AsyncCommand(OnReleaseCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await OnRefreshHoldDishesCommandAsync();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedTable) && SelectedTable is not null)
            {
                HoldDishes = GetHoldDishesByTableNumber(SelectedTable.TableNumber);
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnRefreshHoldDishesCommandAsync()
        {
            IsHoldDishesRefreshing = true;

            _holdDishesSortingType = EHoldDishesSortingType.ByTableNumber;
            HoldDishes = await GetAllHoldDishesAsync();

            Tables = GetHoldTablesFromHoldDishes(HoldDishes);

            IsHoldDishesRefreshing = false;
        }

        private ObservableCollection<HoldDishBindableModel> GetHoldDishesByTableNumber(int tableNumber)
        {
            if (HoldDishes.Any(x => x.TableNumber != tableNumber))
            {
                SelectedHoldDishes = null;
            }

            var holdDishes = _ordersHolding.GetHoldDishesByTableNumber(tableNumber);

            IsNothingFound = !holdDishes.Any();

            return _mapper.Map<ObservableCollection<HoldDishBindableModel>>(holdDishes);
        }

        private Task OnChangeSortHoldDishesCommandAsync(EHoldDishesSortingType holdDishesSortingType)
        {
            if (_holdDishesSortingType == holdDishesSortingType)
            {
                HoldDishes = new(HoldDishes.Reverse());
            }
            else
            {
                _holdDishesSortingType = holdDishesSortingType;

                var sortedHoldDishes = _ordersHolding.GetSortedHoldDishes(_holdDishesSortingType, HoldDishes);

                HoldDishes = new(sortedHoldDishes);
            }

            return Task.CompletedTask;
        }

        private Task OnTapSelectAllHoldDishesTableCommandAsync()
        {
            if (SelectedTable?.TableNumber != Constants.Limits.ALL_TABLES)
            {
                SelectedHoldDishes = SelectedHoldDishes?.Count != HoldDishes.Count
                    ? new(HoldDishes)
                    : null;
            }

            return Task.CompletedTask;
        }

        private Task OnExtendCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnReleaseCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnSetSelectedHoldDishesCommandAsync(List<object>? selectedDishes)
        {
            var selectedCount = selectedDishes?.Count ?? 0;

            if (SelectedHoldDishes?.Count != selectedCount && selectedCount > 0)
            {
                SelectedHoldDishes = new(selectedDishes);
            }

            if (SelectedHoldDishes is not null && selectedCount == 0)
            {
                SelectedHoldDishes = null;
            }

            return Task.CompletedTask;
        }

        private ObservableCollection<TableBindableModel> GetHoldTablesFromHoldDishes(ObservableCollection<HoldDishBindableModel> holdDishes)
        {
            var result = new ObservableCollection<TableBindableModel>();

            if (holdDishes.Any())
            {
                var tables = holdDishes.GroupBy(x => x.TableNumber).Select(y => y.First());

                result = _mapper.Map<ObservableCollection<TableBindableModel>>(tables);

                result.Add(new TableBindableModel { TableNumber = Constants.Limits.ALL_TABLES, });

                result = new(result.OrderBy(x => x.TableNumber));
                SelectedTable = result.FirstOrDefault();
            }

            return result;
        }

        private async Task<ObservableCollection<HoldDishBindableModel>> GetAllHoldDishesAsync()
        {
            var result = await _ordersHolding.GetAllHoldDishesAsync();
            var holdDishes = new ObservableCollection<HoldDishBindableModel>();

            if (result.IsSuccess)
            {
                holdDishes = _mapper.Map<ObservableCollection<HoldDishBindableModel>>(result.Result);
            }
            else
            {
                await _notificationsService.ResponseToBadRequestAsync(result.Exception.Message);
            }

            IsNothingFound = !holdDishes.Any();

            return holdDishes;
        }

        #endregion //--
    }
}

using AutoMapper;
using Next2.Enums;
using Next2.Models.Bindables;
using Next2.Services.DishesHolding;
using Next2.Services.Notifications;
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
    public class HoldDishesViewModel : BaseViewModel
    {
        private readonly IDishesHoldingService _dishesHolding;
        private readonly INotificationsService _notificationsService;
        private readonly IMapper _mapper;

        private EHoldDishesSortingType _holdDishesSortingType;

        public HoldDishesViewModel(
            IDishesHoldingService dishesHolding,
            INotificationsService notificationsService,
            IMapper mapper,
            INavigationService navigationService)
            : base(navigationService)
        {
            _dishesHolding = dishesHolding;
            _notificationsService = notificationsService;
            _mapper = mapper;
        }

        #region -- Public properties --

        public bool IsHoldDishesRefreshing { get; set; }

        public bool IsNothingFound { get; set; }

        public int IndexLastVisibleElement { get; set; }

        public int TableNumber { get; set; }

        public ObservableCollection<HoldDishBindableModel> HoldDishes { get; set; } = new();

        public ObservableCollection<object>? SelectedHoldDishes { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new();

        public TableBindableModel? SelectedTable { get; set; }

        private ICommand? _refreshHoldDishesCommand;
        public ICommand RefreshHoldDishesCommand => _refreshHoldDishesCommand ??= new AsyncCommand(OnRefreshHoldDishesCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _changeSortHoldDishesCommand;
        public ICommand ChangeSortHoldDishesCommand => _changeSortHoldDishesCommand ??= new AsyncCommand<EHoldDishesSortingType>(OnChangeSortHoldDishesCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _setSelectedHoldDishesCommand;
        public ICommand SetSelectedHoldDishesCommand => _setSelectedHoldDishesCommand ??= new AsyncCommand<List<object>>(OnSetSelectedHoldDishesCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _setHoldDishesByTableNumberCommand;
        public ICommand SetHoldDishesByTableNumberCommand => _setHoldDishesByTableNumberCommand ??= new AsyncCommand(OnSetHoldDishesByTableNumberCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _selectAllHoldDishesTableCommand;
        public ICommand SelectAllHoldDishesTableCommand => _selectAllHoldDishesTableCommand ??= new AsyncCommand(OnSelectAllHoldDishesTableCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await OnRefreshHoldDishesCommandAsync();
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

            var holdDishes = _dishesHolding.GetHoldDishesByTableNumber(tableNumber);

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

                var sortedHoldDishes = _dishesHolding.GetSortedHoldDishes(_holdDishesSortingType, HoldDishes);

                HoldDishes = new(sortedHoldDishes);
            }

            return Task.CompletedTask;
        }

        private Task OnSelectAllHoldDishesTableCommandAsync()
        {
            if (SelectedTable?.TableNumber != Constants.Limits.ALL_TABLES)
            {
                SelectedHoldDishes = SelectedHoldDishes?.Count != HoldDishes.Count
                    ? new(HoldDishes)
                    : null;
            }

            return Task.CompletedTask;
        }

        private Task OnSetSelectedHoldDishesCommandAsync(List<object>? selectedDishes)
        {
            var selectedCount = selectedDishes?.Count ?? 0;

            if (selectedCount == 0)
            {
                SelectedHoldDishes = null;
            }
            else if (SelectedHoldDishes?.Count != selectedCount)
            {
                SelectedHoldDishes = new(selectedDishes);
            }

            return Task.CompletedTask;
        }

        private Task OnSetHoldDishesByTableNumberCommandAsync()
        {
            if (SelectedTable is not null)
            {
                TableNumber = SelectedTable.TableNumber;
                HoldDishes = GetHoldDishesByTableNumber(TableNumber);
            }

            return Task.CompletedTask;
        }

        private ObservableCollection<TableBindableModel> GetHoldTablesFromHoldDishes(ObservableCollection<HoldDishBindableModel> holdDishes)
        {
            var bindableTables = new ObservableCollection<TableBindableModel>();

            if (holdDishes.Any())
            {
                var tables = holdDishes.GroupBy(x => x.TableNumber).Select(y => y.First());

                bindableTables = _mapper.Map<ObservableCollection<TableBindableModel>>(tables);

                bindableTables.Add(new TableBindableModel { TableNumber = Constants.Limits.ALL_TABLES, });

                bindableTables = new(bindableTables.OrderBy(x => x.TableNumber));
                SelectedTable = bindableTables.FirstOrDefault();
            }

            return bindableTables;
        }

        private async Task<ObservableCollection<HoldDishBindableModel>> GetAllHoldDishesAsync()
        {
            var result = await _dishesHolding.GetAllHoldDishesAsync();
            var holdDishes = new ObservableCollection<HoldDishBindableModel>();

            if (result.IsSuccess)
            {
                holdDishes = _mapper.Map<ObservableCollection<HoldDishBindableModel>>(result.Result);
            }
            else
            {
                await _notificationsService.ResponseToBadRequestAsync(result.Exception?.Message);
            }

            IsNothingFound = !holdDishes.Any();

            return holdDishes;
        }

        #endregion
    }
}

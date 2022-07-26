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
    public class HoldItemsViewModel : BaseViewModel
    {
        private readonly IOrdersHolding _ordersHolding;
        private readonly INotificationsService _notificationsService;
        private readonly IMapper _mapper;

        private EHoldItemsSortingType _holdItemsSortingType;

        public HoldItemsViewModel(
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

        public bool IsHoldItemsRefreshing { get; set; }

        public bool IsNothingFound { get; set; }

        public int IndexLastVisibleElement { get; set; }

        public ObservableCollection<HoldItemBindableModel> HoldItems { get; set; } = new();

        public ObservableCollection<object>? SelectedHoldItems { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new();

        public TableBindableModel? SelectedTable { get; set; }

        private ICommand _refreshHoldItemsCommand;
        public ICommand RefreshHoldItemsCommand => _refreshHoldItemsCommand ??= new AsyncCommand(OnRefreshHoldItemsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changeSortHoldItemsCommand;
        public ICommand ChangeSortHoldItemsCommand => _changeSortHoldItemsCommand ??= new AsyncCommand<EHoldItemsSortingType>(OnChangeSortHoldItemsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _setSelectedHoldItemsCommand;
        public ICommand SetSelectedHoldItemsCommand => _setSelectedHoldItemsCommand ??= new AsyncCommand<List<object>?>(OnSetSelectedHoldItemsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapSelectAllHoldItemsTableCommand;
        public ICommand TapSelectAllHoldItemsTableCommand => _tapSelectAllHoldItemsTableCommand ??= new AsyncCommand(OnTapSelectAllHoldItemsTableCommandAsync, allowsMultipleExecutions: false);

        private ICommand _extendCommand;
        public ICommand ExtendCommand => _extendCommand ??= new AsyncCommand(OnExtendCommandAsync, allowsMultipleExecutions: false);

        private ICommand _releaseCommand;
        public ICommand ReleaseCommand => _releaseCommand ??= new AsyncCommand(OnReleaseCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await OnRefreshHoldItemsCommandAsync();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedTable) && SelectedTable is not null)
            {
                HoldItems = GetHoldItemsByTableNumber(SelectedTable.TableNumber);
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnRefreshHoldItemsCommandAsync()
        {
            IsHoldItemsRefreshing = true;

            _holdItemsSortingType = EHoldItemsSortingType.ByTableName;
            HoldItems = await GetAllHoldItemsAsync();

            Tables = GetHoldTablesFromHoldItems(HoldItems);

            IsHoldItemsRefreshing = false;
        }

        private ObservableCollection<HoldItemBindableModel> GetHoldItemsByTableNumber(int tableNumber)
        {
            if (HoldItems.Any(x => x.TableNumber != tableNumber))
            {
                SelectedHoldItems = null;
            }

            var holdItems = _ordersHolding.GetHoldItemsByTableNumber(tableNumber);

            IsNothingFound = !holdItems.Any();

            return _mapper.Map<ObservableCollection<HoldItemBindableModel>>(holdItems);
        }

        private Task OnChangeSortHoldItemsCommandAsync(EHoldItemsSortingType holdItemsSortingType)
        {
            if (_holdItemsSortingType == holdItemsSortingType)
            {
                HoldItems = new(HoldItems.Reverse());
            }
            else
            {
                _holdItemsSortingType = holdItemsSortingType;

                var sortedHoldItems = _ordersHolding.GetSortedHoldItems(_holdItemsSortingType, HoldItems);

                HoldItems = new(sortedHoldItems);
            }

            return Task.CompletedTask;
        }

        private Task OnTapSelectAllHoldItemsTableCommandAsync()
        {
            if (SelectedTable?.TableNumber != Constants.Limits.ALL_TABLES)
            {
                SelectedHoldItems = SelectedHoldItems?.Count == HoldItems.Count
                    ? null
                    : new(HoldItems);
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

        private Task OnSetSelectedHoldItemsCommandAsync(List<object>? selectedItems)
        {
            var selectedCount = selectedItems?.Count ?? 0;

            if (SelectedHoldItems?.Count != selectedCount && selectedCount > 0)
            {
                SelectedHoldItems = new(selectedItems);
            }

            if (SelectedHoldItems is not null && selectedCount == 0)
            {
                SelectedHoldItems = null;
            }

            return Task.CompletedTask;
        }

        private ObservableCollection<TableBindableModel> GetHoldTablesFromHoldItems(ObservableCollection<HoldItemBindableModel> holdItems)
        {
            var result = new ObservableCollection<TableBindableModel>();

            if (holdItems.Any())
            {
                var tables = holdItems.GroupBy(x => x.TableNumber).Select(y => y.First());

                result = _mapper.Map<ObservableCollection<TableBindableModel>>(tables);

                result.Add(new TableBindableModel { TableNumber = Constants.Limits.ALL_TABLES, });

                result = new(result.OrderBy(x => x.TableNumber));
                SelectedTable = result.FirstOrDefault();
            }

            return result;
        }

        private async Task<ObservableCollection<HoldItemBindableModel>> GetAllHoldItemsAsync()
        {
            var resultHoldItems = await _ordersHolding.GetAllHoldItemsAsync();
            var result = new ObservableCollection<HoldItemBindableModel>();

            if (resultHoldItems.IsSuccess)
            {
                result = _mapper.Map<ObservableCollection<HoldItemBindableModel>>(resultHoldItems.Result);
            }
            else
            {
                await _notificationsService.ResponseToBadRequestAsync(resultHoldItems.Exception.Message);
            }

            IsNothingFound = !result.Any();

            return result;
        }

        #endregion
    }
}

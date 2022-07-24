using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Models.Bindables;
using Next2.Services.HoldItem;
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
        private readonly IHoldItemService _holdItemService;
        private readonly INotificationsService _notificationsService;
        private readonly IMapper _mapper;

        private EHoldItemsSortingType _holdItemsSortingType;

        public HoldItemsViewModel(
            IHoldItemService holdItemService,
            INotificationsService notificationsService,
            IMapper mapper,
            INavigationService navigationService)
            : base(navigationService)
        {
            _holdItemService = holdItemService;
            _notificationsService = notificationsService;
            _mapper = mapper;
        }

        #region -- Public properties --

        public bool IsHoldItemsRefreshing { get; set; }

        public bool IsNothingFound { get; set; }

        public int IndexLastVisibleElement { get; set; }

        public int IndexLastItemDisplay { get; set; }

        public ObservableCollection<HoldItemBindableModel> HoldItems { get; set; } = new();

        private ObservableCollection<object>? _selectedHoldItems;
        public ObservableCollection<object>? SelectedHoldItems
        {
            get => _selectedHoldItems;
            set
            {
                if (_selectedHoldItems != value)
                {
                    _selectedHoldItems = value;
                }
            }
        }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new();

        public TableBindableModel? SelectedTable { get; set; }

        private ICommand _refreshHoldItemsCommand;
        public ICommand RefreshHoldItemsCommand => _refreshHoldItemsCommand ??= new AsyncCommand(OnRefreshHoldItemsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changeSortHoldItemsCommand;
        public ICommand ChangeSortHoldItemsCommand => _changeSortHoldItemsCommand ??= new AsyncCommand<EHoldItemsSortingType>(OnChangeSortHoldItemsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _getSelectedHoldItemsCommand;
        public ICommand GetSelectedHoldItemsCommand => _getSelectedHoldItemsCommand ??= new AsyncCommand<List<object>?>(OnGetSelectedHoldItemsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapSelectAllHoldItemsTableCommand;
        public ICommand TapSelectAllHoldItemsTableCommand => _tapSelectAllHoldItemsTableCommand ??= new AsyncCommand(OnTapSelectAllHoldItemsTableCommand, allowsMultipleExecutions: false);

        private ICommand _extendCommand;
        public ICommand ExtendCommand => _extendCommand ??= new AsyncCommand(OnExtendCommand, allowsMultipleExecutions: false);

        private ICommand _releaseCommand;
        public ICommand ReleaseCommand => _releaseCommand ??= new AsyncCommand(OnReleaseCommand, allowsMultipleExecutions: false);

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
                //HoldItems = GetHoldItemsByTableNumber(SelectedTable.TableNumber);
            }

            if (args.PropertyName is nameof(IndexLastVisibleElement))
            {
                if (IndexLastVisibleElement > IndexLastItemDisplay)
                {
                    IndexLastItemDisplay = IndexLastVisibleElement;
                }
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnRefreshHoldItemsCommandAsync()
        {
            IsHoldItemsRefreshing = true;

            _holdItemsSortingType = EHoldItemsSortingType.ByTableName;
            HoldItems = await GetFullHoldItemsAsync();

            Tables = GetHoldTablesFromHoldItems(HoldItems);

            IsHoldItemsRefreshing = false;
        }

        private ObservableCollection<HoldItemBindableModel> GetHoldItemsByTableNumber(int tableNumber)
        {
            if (HoldItems.FirstOrDefault()?.TableNumber != tableNumber)
            {
                SelectedHoldItems = null;
            }

            var holdItems = _holdItemService.GetHoldItemsByTableNumber(tableNumber);

            IsNothingFound = !holdItems.Any();
            IndexLastItemDisplay = HoldItems.Count;

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

                var sortedHoldItems = _holdItemService.GetSortedHoldItems(_holdItemsSortingType, HoldItems);

                HoldItems = new(sortedHoldItems);
            }

            return Task.CompletedTask;
        }

        private Task OnTapSelectAllHoldItemsTableCommand()
        {
            if (SelectedTable?.TableNumber != Constants.Limits.ALL_TABLES)
            {
                if (SelectedHoldItems?.Count == HoldItems.Count)
                {
                    SelectedHoldItems = null;
                }
                else
                {
                    SelectedHoldItems = new(HoldItems);
                }
            }

            return Task.CompletedTask;
        }

        private Task OnExtendCommand()
        {
            return Task.CompletedTask;
        }

        private Task OnReleaseCommand()
        {
            return Task.CompletedTask;
        }

        private Task OnGetSelectedHoldItemsCommandAsync(List<object>? selectedItems)
        {
            var selectedCount = selectedItems?.Count;

            if (SelectedHoldItems?.Count != selectedCount && selectedCount != 0)
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

                if (App.IsTablet)
                {
                    result.Add(new TableBindableModel { TableNumber = Constants.Limits.ALL_TABLES, });
                }

                result = new(result.OrderBy(x => x.TableNumber));
                SelectedTable = result.FirstOrDefault();
            }

            return result;
        }

        private async Task<ObservableCollection<HoldItemBindableModel>> GetFullHoldItemsAsync()
        {
            var holdItems = await _holdItemService.GetAllHoldItemsAsync();
            var result = new ObservableCollection<HoldItemBindableModel>();

            if (holdItems.IsSuccess)
            {
                result = _mapper.Map<ObservableCollection<HoldItemBindableModel>>(holdItems.Result);
            }
            else
            {
                await _notificationsService.ResponseToBadRequestAsync(holdItems.Exception.Message);
            }

            IsNothingFound = !result.Any();
            IndexLastItemDisplay = HoldItems.Count;

            return result;
        }

        #endregion
    }
}

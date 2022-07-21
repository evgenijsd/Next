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

        public ObservableCollection<HoldItemBindableModel> HoldItems { get; set; } = new();

        public ObservableCollection<HoldItemBindableModel>? SelectedHoldItems { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new();

        public TableBindableModel? SelectedTable { get; set; }

        private ICommand _refreshHoldItemsCommand;
        public ICommand RefreshHoldItemsCommand => _refreshHoldItemsCommand ??= new AsyncCommand(OnRefreshHoldItemsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changeSortHoldItemsCommand;
        public ICommand ChangeSortHoldItemsCommand => _changeSortHoldItemsCommand ??= new AsyncCommand<EHoldItemsSortingType>(OnChangeSortHoldItemsCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            IsHoldItemsRefreshing = true;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            HoldItems = new();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(SelectedTable))
            {
                int i = 0;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnRefreshHoldItemsCommandAsync()
        {
            _holdItemsSortingType = EHoldItemsSortingType.None;
            HoldItems = await GetHoldItemsAsync();

            Tables = GetHoldTables(HoldItems);
            SelectedTable = Tables.FirstOrDefault();

            //HoldItems = new(HoldItems.Where(x => x.Id < 9));
            await OnChangeSortHoldItemsCommandAsync(EHoldItemsSortingType.ByTableName);

            IsHoldItemsRefreshing = false;
            IsNothingFound = !HoldItems.Any();
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

        private ObservableCollection<TableBindableModel> GetHoldTables(ObservableCollection<HoldItemBindableModel> holdItems)
        {
            var result = new ObservableCollection<TableBindableModel>();

            if (holdItems.Any())
            {
                var tables = holdItems.GroupBy(x => x.TableNumber).Select(y => y.First());

                result = _mapper.Map<ObservableCollection<TableBindableModel>>(tables);
                result.Add(new TableBindableModel { TableNumber = 0, });
                foreach (var table in result)
                {
                    table.Id = Guid.NewGuid();
                }

                result = new(result.OrderBy(x => x.TableNumber));
            }

            return result;
        }

        private async Task<ObservableCollection<HoldItemBindableModel>> GetHoldItemsAsync()
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

            return result;
        }

        #endregion
    }
}

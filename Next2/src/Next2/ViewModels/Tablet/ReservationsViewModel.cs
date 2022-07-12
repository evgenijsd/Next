using Next2.Models.Bindables;
using Next2.Services.Reservation;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet
{
    public class ReservationsViewModel : BaseViewModel
    {
        private readonly IReservationService _reservationService;

        public ReservationsViewModel(
            IReservationService reservationService,
            INavigationService navigationService)
            : base(navigationService)
        {
            _reservationService = reservationService;
        }

        #region -- Public properties --

        public string SearchQuery { get; set; } = string.Empty;

        public bool IsReservationsRefreshing { get; set; }

        public bool IsNothingFound => !string.IsNullOrEmpty(SearchQuery) && !Reservations.Any();

        public bool IsTabsModeSelected { get; set; }

        public bool IsPreloadStateActive => !string.IsNullOrEmpty(SearchQuery) && (!IsInternetConnected || (IsReservationsRefreshing && !Reservations.Any()));

        public ReservationBindableModel? SelectedReservation { get; set; }

        public ObservableCollection<ReservationBindableModel> Reservations { get; set; } = new();

        private ICommand _goToSearchQueryInputCommand;
        public ICommand GoToSearchQueryInputCommand => _goToSearchQueryInputCommand ??= new AsyncCommand(OnGoToSearchQueryInputCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearSearchCommand;
        public ICommand ClearSearchResultCommand => _clearSearchCommand ??= new AsyncCommand(OnClearSearchResultCommandAsync);

        private ICommand _refreshOrdersCommand;
        public ICommand RefreshOrdersCommand => _refreshOrdersCommand ??= new AsyncCommand(OnRefreshOrdersCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changeSortOrderCommand;
        public ICommand ChangeSortOrderCommand => _changeSortOrderCommand ??= new AsyncCommand(OnChangeSortOrderCommand, allowsMultipleExecutions: false);

        private ICommand _selectOrderCommand;
        public ICommand SelectOrderCommand => _selectOrderCommand ??= new AsyncCommand(OnSelectOrderCommand);

        #endregion

        #region -- Private helpers --

        private async Task OnGoToSearchQueryInputCommandAsync()
        {
            //if (Orders.Any() || !string.IsNullOrEmpty(SearchQuery))
            //{
            //    Func<string, string> searchValidator = _orderService.ApplyNameFilter;

            //    var searchHint = IsTabsModeSelected
            //        ? LocalizationResourceManager.Current["NameOrOrder"]
            //        : LocalizationResourceManager.Current["TableNumberOrOrder"];

            //    var parameters = new NavigationParameters()
            //    {
            //        { Constants.Navigations.SEARCH, SearchQuery },
            //        { Constants.Navigations.FUNC, searchValidator },
            //        { Constants.Navigations.PLACEHOLDER, searchHint },
            //    };

            //    IsSearchModeActive = true;

            //    await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
            //}
        }

        private async Task OnClearSearchResultCommandAsync()
        {
            if (SearchQuery != string.Empty)
            {
                SearchQuery = string.Empty;
                //IsSearchModeActive = false;
                //IsOrdersRefreshing = true;
            }
            else
            {
                await OnGoToSearchQueryInputCommandAsync();
            }
        }

        private Task OnRefreshOrdersCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnChangeSortOrderCommand()
        {
            //if (OrderSortingType == orderSortingType)
            //{
            //    Orders = new(Orders.Reverse());
            //}
            //else
            //{
            //    OrderSortingType = orderSortingType;

            //    Orders = new(GetSortedOrders(Orders));
            //}
            return Task.CompletedTask;
        }

        private Task OnSelectOrderCommand()
        {
            //SelectedOrder = order == SelectedOrder ? null : order;
            return Task.CompletedTask;
        }

        #endregion
    }
}

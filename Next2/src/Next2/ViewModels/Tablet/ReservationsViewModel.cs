using AutoMapper;
using Next2.Enums;
using Next2.Models.Bindables;
using Next2.Services.Reservation;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet
{
    public class ReservationsViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly IReservationService _reservationService;

        public ReservationsViewModel(
            IReservationService reservationService,
            IMapper mapper,
            INavigationService navigationService)
            : base(navigationService)
        {
            _mapper = mapper;
            _reservationService = reservationService;
        }

        #region -- Public properties --

        public string SearchQuery { get; set; } = string.Empty;

        public bool IsReservationsRefreshing { get; set; }

        public bool IsNothingFound => !string.IsNullOrEmpty(SearchQuery) && !Reservations.Any();

        public bool IsPreloadStateActive => !string.IsNullOrEmpty(SearchQuery) && (!IsInternetConnected || (IsReservationsRefreshing && !Reservations.Any()));

        public EReservationsSortingType ReservationSortingType { get; set; }

        public ReservationBindableModel? SelectedReservation { get; set; }

        public ObservableCollection<ReservationBindableModel> Reservations { get; set; } = new();

        private ICommand _goToSearchQueryInputCommand;
        public ICommand GoToSearchQueryInputCommand => _goToSearchQueryInputCommand ??= new AsyncCommand(OnGoToSearchQueryInputCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearSearchCommand;
        public ICommand ClearSearchResultCommand => _clearSearchCommand ??= new AsyncCommand(OnClearSearchResultCommandAsync, allowsMultipleExecutions: false);

        private ICommand _refreshReservationsCommand;
        public ICommand RefreshReservationsCommand => _refreshReservationsCommand ??= new AsyncCommand(OnRefreshReservationsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changeSortReservationCommand;
        public ICommand ChangeSortReservationCommand => _changeSortReservationCommand ??= new AsyncCommand<EReservationsSortingType>(OnChangeSortReservationCommand, allowsMultipleExecutions: false);

        private ICommand _selectReservationCommand;
        public ICommand SelectReservationCommand => _selectReservationCommand ??= new AsyncCommand(OnSelectReservationCommand);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            IsReservationsRefreshing = true;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            SearchQuery = string.Empty;
            SelectedReservation = null;
            Reservations = new();
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.SEARCH_QUERY, out string searchQuery))
            {
                SearchQuery = searchQuery;
                IsReservationsRefreshing = true;
            }
        }

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
            IsReservationsRefreshing = true;
        }

        private async Task OnRefreshReservationsCommandAsync()
        {
            var resultOfGettingReservations = await _reservationService.GetReservationsListAsync(SearchQuery);

            if (resultOfGettingReservations.IsSuccess)
            {
                var reservations = _mapper.Map<ObservableCollection<ReservationBindableModel>>(resultOfGettingReservations.Result);

                Reservations = new(reservations);
            }
        }

        private Task OnChangeSortReservationCommand(EReservationsSortingType reservationsSortingType)
        {
            if (ReservationSortingType == reservationsSortingType)
            {
                Reservations = new(Reservations.Reverse());
            }
            else
            {
                ReservationSortingType = reservationsSortingType;

                Reservations = new(GetSortedOrders(Reservations));
            }

            return Task.CompletedTask;
        }

        private IEnumerable<ReservationBindableModel> GetSortedOrders(IEnumerable<ReservationBindableModel> reservations)
        {
            Func<ReservationBindableModel, object> sortingSelector = ReservationSortingType switch
            {
                EReservationsSortingType.ByCustomerName => x => x.CustomerName,
                EReservationsSortingType.ByTableNumber => x => x.TableNumber,
                _ => throw new NotImplementedException(),
            };

            return reservations.OrderBy(sortingSelector);
        }

        private Task OnSelectReservationCommand()
        {
            //SelectedOrder = order == SelectedOrder ? null : order;
            return Task.CompletedTask;
        }

        #endregion
    }
}

using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Reservation;
using Next2.Views.Mobile;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
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

        public ReservationModel? SelectedReservation { get; set; }

        public ObservableCollection<ReservationModel> Reservations { get; set; } = new();

        private ICommand _goToSearchQueryInputCommand;
        public ICommand GoToSearchQueryInputCommand => _goToSearchQueryInputCommand ??= new AsyncCommand(OnGoToSearchQueryInputCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearSearchCommand;
        public ICommand ClearSearchResultCommand => _clearSearchCommand ??= new AsyncCommand(OnClearSearchResultCommandAsync, allowsMultipleExecutions: false);

        private ICommand _refreshReservationsCommand;
        public ICommand RefreshReservationsCommand => _refreshReservationsCommand ??= new AsyncCommand(OnRefreshReservationsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changeSortReservationCommand;
        public ICommand ChangeSortReservationCommand => _changeSortReservationCommand ??= new AsyncCommand<EReservationsSortingType>(OnChangeSortReservationCommand, allowsMultipleExecutions: false);

        private ICommand _selectReservationCommand;
        public ICommand SelectReservationCommand => _selectReservationCommand ??= new AsyncCommand<ReservationModel>(OnSelectReservationCommand);

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

        #region -- Public helpers --

        public void SetSearchQuery(string searchQuery)
        {
            SearchQuery = searchQuery;

            IsReservationsRefreshing = true;
        }

        #endregion

        #region -- Private helpers --

        private async Task OnGoToSearchQueryInputCommandAsync()
        {
            Func<string, string> searchValidator = Filters.ApplyNameFilter;

            var parameters = new NavigationParameters()
            {
                { Constants.Navigations.SEARCH_RESERVATION, SearchQuery },
                { Constants.Navigations.FUNC, searchValidator },
                { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["SearchByNameOrPhone"] },
            };

            await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
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
                Reservations = new(GetSortedOrders(resultOfGettingReservations.Result));
            }
            else
            {
                await ResponseToBadRequestAsync(resultOfGettingReservations.Exception.Message);
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

        private IEnumerable<ReservationModel> GetSortedOrders(IEnumerable<ReservationModel> reservations)
        {
            Func<ReservationModel, object> sortingSelector = ReservationSortingType switch
            {
                EReservationsSortingType.ByCustomerName => x => x.CustomerName,
                EReservationsSortingType.ByTableNumber => x => x.TableNumber,
                _ => throw new NotImplementedException(),
            };

            return reservations.OrderBy(sortingSelector);
        }

        private Task OnSelectReservationCommand(ReservationModel reservation)
        {
            SelectedReservation = reservation == SelectedReservation
                ? null
                : reservation;

            return Task.CompletedTask;
        }

        #endregion
    }
}

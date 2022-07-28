using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Notifications;
using Next2.Services.Reservation;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
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
        private readonly INotificationsService _notificationsService;

        private EReservationsSortingType _reservationsSortingType;

        public ReservationsViewModel(
            IReservationService reservationService,
            IMapper mapper,
            INotificationsService notificationsService,
            INavigationService navigationService)
            : base(navigationService)
        {
            _mapper = mapper;
            _reservationService = reservationService;
            _notificationsService = notificationsService;
        }

        #region -- Public properties --

        public string SearchQuery { get; set; } = string.Empty;

        public bool IsReservationsRefreshing { get; set; }

        public bool IsNothingFound => !string.IsNullOrEmpty(SearchQuery) && !Reservations.Any();

        public bool IsPreloadStateActive => !string.IsNullOrEmpty(SearchQuery) && (!IsInternetConnected || (IsReservationsRefreshing && !Reservations.Any()));

        public ReservationModel? SelectedReservation { get; set; }

        public ObservableCollection<ReservationModel> Reservations { get; set; } = new();

        private ICommand _goToSearchQueryInputCommand;
        public ICommand GoToSearchQueryInputCommand => _goToSearchQueryInputCommand ??= new AsyncCommand(OnGoToSearchQueryInputCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearSearchResultCommand;
        public ICommand ClearSearchResultCommand => _clearSearchResultCommand ??= new AsyncCommand(OnClearSearchResultCommandAsync, allowsMultipleExecutions: false);

        private ICommand _refreshReservationsCommand;
        public ICommand RefreshReservationsCommand => _refreshReservationsCommand ??= new AsyncCommand(OnRefreshReservationsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changeSortReservationCommand;
        public ICommand ChangeSortReservationCommand => _changeSortReservationCommand ??= new AsyncCommand<EReservationsSortingType>(OnChangeSortReservationCommandAsync, allowsMultipleExecutions: false);

        private ICommand _selectReservationCommand;
        public ICommand SelectReservationCommand => _selectReservationCommand ??= new AsyncCommand<ReservationModel>(OnSelectReservationCommandAsync, allowsMultipleExecutions: false);

        private ICommand _addNewReservationCommand;
        public ICommand AddNewReservationCommand => _addNewReservationCommand ??= new AsyncCommand(OnAddNewReservationCommandAsync, allowsMultipleExecutions: false);

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
                SetSearchQuery(searchQuery);
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

        private Task OnGoToSearchQueryInputCommandAsync()
        {
            Func<string, string> searchValidator = Filters.StripInvalidNameCharacters;

            var parameters = new NavigationParameters()
            {
                { Constants.Navigations.SEARCH_RESERVATION, SearchQuery },
                { Constants.Navigations.FUNC, searchValidator },
                { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["SearchByNameOrPhone"] },
            };

            return _navigationService.NavigateAsync(nameof(SearchPage), parameters);
        }

        private Task OnClearSearchResultCommandAsync()
        {
            IsReservationsRefreshing = true;

            return Task.CompletedTask;
        }

        private async Task OnRefreshReservationsCommandAsync()
        {
            var resultOfGettingReservations = await _reservationService.GetReservationsAsync(SearchQuery);

            if (resultOfGettingReservations.IsSuccess)
            {
                SelectedReservation = null;

                var sortedReservations = _reservationService.GetSortedReservations(_reservationsSortingType, resultOfGettingReservations.Result);

                Reservations = new(sortedReservations);
            }
            else
            {
                await _notificationsService.ResponseToBadRequestAsync(resultOfGettingReservations.Exception.Message);
            }
        }

        private Task OnChangeSortReservationCommandAsync(EReservationsSortingType reservationsSortingType)
        {
            if (_reservationsSortingType == reservationsSortingType)
            {
                Reservations = new(Reservations.Reverse());
            }
            else
            {
                _reservationsSortingType = reservationsSortingType;

                var sortedReservations = _reservationService.GetSortedReservations(_reservationsSortingType, Reservations);

                Reservations = new(sortedReservations);
            }

            return Task.CompletedTask;
        }

        private Task OnSelectReservationCommandAsync(ReservationModel reservation)
        {
            SelectedReservation = reservation == SelectedReservation
                ? null
                : reservation;

            return Task.CompletedTask;
        }

        private Task OnAddNewReservationCommandAsync()
        {
            var param = new DialogParameters();

            var popupPage = new Views.Tablet.Dialogs.AddNewReservationDialog(param, AddNewReservationDialogCallBack);

            return PopupNavigation.PushAsync(popupPage);
        }

        private async void AddNewReservationDialogCallBack(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out ReservationModel reservation))
            {
                var resultOfAddingReservation = await _reservationService.AddReservationAsync(reservation);

                if (resultOfAddingReservation.IsSuccess)
                {
                    await _notificationsService.CloseAllPopupAsync();

                    IsReservationsRefreshing = true;
                }
                else
                {
                    var message = resultOfAddingReservation.Exception?.Message;

                    await _notificationsService.ResponseToBadRequestAsync(message);
                }
            }
            else
            {
                await _notificationsService.CloseAllPopupAsync();
            }
        }

        #endregion
    }
}

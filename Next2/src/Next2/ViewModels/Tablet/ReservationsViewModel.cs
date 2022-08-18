using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Authentication;
using Next2.Services.Notifications;
using Next2.Services.Reservation;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
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

        private EReservationsSortingType _reservationsSortingType;

        public ReservationsViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            IReservationService reservationService,
            IMapper mapper)
            : base(navigationService, authenticationService, notificationsService)
        {
            _mapper = mapper;
            _reservationService = reservationService;
        }

        #region -- Public properties --

        public string SearchQuery { get; set; } = string.Empty;

        public bool IsReservationsRefreshing { get; set; }

        public bool IsNothingFound => !string.IsNullOrEmpty(SearchQuery) && !Reservations.Any();

        public bool IsPreloadStateActive => !string.IsNullOrEmpty(SearchQuery) && (!IsInternetConnected || (IsReservationsRefreshing && !Reservations.Any()));

        public ReservationModel? SelectedReservation { get; set; }

        public ObservableCollection<ReservationModel> Reservations { get; set; } = new();

        private ICommand? _goToSearchQueryInputCommand;
        public ICommand GoToSearchQueryInputCommand => _goToSearchQueryInputCommand ??= new AsyncCommand(OnGoToSearchQueryInputCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _clearSearchResultCommand;
        public ICommand ClearSearchResultCommand => _clearSearchResultCommand ??= new AsyncCommand(OnClearSearchResultCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _refreshReservationsCommand;
        public ICommand RefreshReservationsCommand => _refreshReservationsCommand ??= new AsyncCommand(OnRefreshReservationsCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _changeSortReservationCommand;
        public ICommand ChangeSortReservationCommand => _changeSortReservationCommand ??= new AsyncCommand<EReservationsSortingType>(OnChangeSortReservationCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _selectReservationCommand;
        public ICommand SelectReservationCommand => _selectReservationCommand ??= new AsyncCommand<ReservationModel>(OnSelectReservationCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _addNewReservationCommand;
        public ICommand AddNewReservationCommand => _addNewReservationCommand ??= new AsyncCommand(OnAddNewReservationCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _assignReservationCommand;
        public ICommand AssignReservationCommand => _assignReservationCommand ??= new AsyncCommand(OnAssignReservationCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _removeReservationCommand;
        public ICommand RemoveReservationCommand => _removeReservationCommand ??= new AsyncCommand(OnRemoveReservationCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _infoAboutReservationCommand;
        public ICommand InfoAboutReservationCommand => _infoAboutReservationCommand ??= new AsyncCommand(OnInfoAboutReservationCommandAsync, allowsMultipleExecutions: false);

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
                await ResponseToUnsuccessfulRequestAsync(resultOfGettingReservations.Exception?.Message);
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

        private Task OnSelectReservationCommandAsync(ReservationModel? reservation)
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
                    await _notificationsService.ClosePopupAsync();

                    IsReservationsRefreshing = true;
                }
                else
                {
                    await ResponseToUnsuccessfulRequestAsync(resultOfAddingReservation.Exception?.Message);
                }
            }
            else
            {
                await _notificationsService.ClosePopupAsync();
            }
        }

        private Task OnRemoveReservationCommandAsync()
        {
            var confirmDialogParameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["ThisReservationWillBeRemoved"] },
                { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Remove"] },
            };

            PopupPage confirmDialog = new Views.Tablet.Dialogs.ConfirmDialog(confirmDialogParameters, CloseRemoveReservationDialogCallBack);

            return PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseRemoveReservationDialogCallBack(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool accept) && accept && SelectedReservation is not null)
            {
                var reservationRemovingResult = await _reservationService.RemoveReservationAsync(SelectedReservation);

                if (reservationRemovingResult.IsSuccess)
                {
                    IsReservationsRefreshing = true;

                    await _notificationsService.CloseAllPopupAsync();
                }
                else
                {
                    await ResponseToUnsuccessfulRequestAsync(reservationRemovingResult.Exception?.Message);
                }
            }
            else
            {
                await _notificationsService.ClosePopupAsync();
            }
        }

        private Task OnInfoAboutReservationCommandAsync()
        {
            var confirmDialogParameters = new DialogParameters
            {
                { Constants.DialogParameterKeys.MODEL, SelectedReservation },
            };

            PopupPage confirmDialog = new Views.Tablet.Dialogs.InfoAboutReservationDialog(confirmDialogParameters, CloseInfoAboutReservationDialogCallBack);

            return PopupNavigation.PushAsync(confirmDialog);
        }

        private async void CloseInfoAboutReservationDialogCallBack(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.ACTION, out string action))
            {
                switch (action)
                {
                    case Constants.DialogParameterKeys.REMOVE:
                        await OnRemoveReservationCommandAsync();

                        break;
                }
            }
            else
            {
                await _notificationsService.ClosePopupAsync();
            }
        }

        private Task OnAssignReservationCommandAsync()
        {
            var popupPage = new Views.Tablet.Dialogs.AssignReservationDialog(CloseAssignReservationDialogCallBack);

            return PopupNavigation.PushAsync(popupPage);
        }

        private async void CloseAssignReservationDialogCallBack(IDialogParameters param)
        {
        }

        #endregion
    }
}

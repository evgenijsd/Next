using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Customers;
using Next2.Services.Reservation;
using Next2.Views.Mobile;
using Prism.Navigation;
using Xamarin.CommunityToolkit.Extensions;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.UI.Views;
using Prism.Navigation.Xaml;
using Next2.Views;
using Prism;

namespace Next2.ViewModels.Tablet
{
    public class ReservationsViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly IReservationService _reservationService;

        private EReservationsSortingType _reservationsSortingType;

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
        public ICommand SelectReservationCommand => _selectReservationCommand ??= new AsyncCommand<ReservationModel>(OnSelectReservationCommandAsync);

        private ICommand _addNewReservationCommand;
        public ICommand AddNewReservationCommand => _addNewReservationCommand ??= new AsyncCommand(OnAddNewReservationCommandAsync);

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

        public void SetInputNotes(string inputNotes)
        {
            MessagingCenter.Send(this, Constants.Navigations.INPUT_NOTES, inputNotes);
        }

        #endregion

        #region -- Private helpers --

        private Task OnGoToSearchQueryInputCommandAsync()
        {
            Func<string, string> searchValidator = Filters.StripInvalidNameCharacters;

            var parameters = new Prism.Navigation.NavigationParameters()
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
                await ResponseToBadRequestAsync(resultOfGettingReservations.Exception.Message);
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

        private INavigation Navigation => App.Current.MainPage.Navigation;

        private async Task OnAddNewReservationCommandAsync()
        {
            //Navigation.ShowPopup(new Views.Tablet.Dialogs.NoLightDismissPopup());

            //var navigationParameters = new Prism.Navigation.NavigationParameters()
            //{
            //    { Constants.Navigations.INPUT_NOTES, "Notes" },
            //    { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["CommentForReservation"] },
            //};

            //await _navigationService.NavigateAsync(nameof(SearchPage), navigationParameters);
            var page = new BaseContentPage()
            {
                BackgroundColor = Color.FromRgba(1, 0, 0, 0.5),
                Content = new StackLayout()
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = 500,
                    WidthRequest = 500,
                    Children =
                    {
                        new Label()
                        {
                            Text = "sddssd",
                        },
                    },
                },
            };

            //await Navigation.PushModalAsync(page);
            var param = new DialogParameters();

            var popupPage = new Views.Tablet.Dialogs.AddNewReservationDialog(param, AddNewReservationDialogCallBack);

            await PopupNavigation.PushAsync(popupPage);
            //await Navigation.PushModalAsync(popupPage);
            IDialogService dialogService = App.Resolve<IDialogService>();

            //dialogService.ShowDialog(nameof(Views.Tablet.Dialogs.DialogView));

            //var navigationParameters = new Prism.Navigation.NavigationParameters()
            //{
            //    { Constants.Navigations.INPUT_NOTES, "df" },
            //    { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["CommentForReservation"] },
            //};

            //await _navigationService.NavigateAsync(nameof(SearchPage), navigationParameters, true);
            //var param = new DialogParameters();

            //var popupPage = new Views.Tablet.Dialogs.AddNewReservationDialog(param, AddNewReservationDialogCallBack, _navigationService, App.Resolve<ICustomersService>());

            //return PopupNavigation.PushAsync(popupPage);
        }

        private async void AddNewReservationDialogCallBack(IDialogParameters param)
        {
            if (param.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out Guid customerId))
            {
            }
        }

        #endregion
    }
}

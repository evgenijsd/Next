﻿using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Authentication;
using Next2.Services.Employees;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Services.Reservation;
using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
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
        private readonly IEmployeesService _employeesService;
        private readonly IOrderService _orderService;

        private EReservationsSortingType _reservationsSortingType;

        public ReservationsViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            IReservationService reservationService,
            IEmployeesService employeesService,
            IOrderService orderService,
            IMapper mapper)
            : base(navigationService, authenticationService, notificationsService)
        {
            _mapper = mapper;
            _reservationService = reservationService;
            _employeesService = employeesService;
            _orderService = orderService;
        }

        #region -- Public properties --

        public string SearchQuery { get; set; } = string.Empty;

        public bool IsReservationsRefreshing { get; set; }

        public bool IsNothingFound => !string.IsNullOrEmpty(SearchQuery) && !Reservations.Any();

        public bool IsPreloadStateActive => !string.IsNullOrEmpty(SearchQuery) && (!IsInternetConnected || (IsReservationsRefreshing && !Reservations.Any()));

        public ReservationBindableModel? SelectedReservation { get; set; }

        public ObservableCollection<ReservationBindableModel> Reservations { get; set; } = new();

        private ICommand? _goToSearchQueryInputCommand;
        public ICommand GoToSearchQueryInputCommand => _goToSearchQueryInputCommand ??= new AsyncCommand(OnGoToSearchQueryInputCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _clearSearchResultCommand;
        public ICommand ClearSearchResultCommand => _clearSearchResultCommand ??= new AsyncCommand(OnClearSearchResultCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _refreshReservationsCommand;
        public ICommand RefreshReservationsCommand => _refreshReservationsCommand ??= new AsyncCommand(OnRefreshReservationsCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _changeSortReservationCommand;
        public ICommand ChangeSortReservationCommand => _changeSortReservationCommand ??= new AsyncCommand<EReservationsSortingType>(OnChangeSortReservationCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _selectReservationCommand;
        public ICommand SelectReservationCommand => _selectReservationCommand ??= new AsyncCommand<ReservationBindableModel>(OnSelectReservationCommandAsync, allowsMultipleExecutions: false);

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

                var allReservations = _mapper.Map<IEnumerable<ReservationBindableModel>>(resultOfGettingReservations.Result);

                var sortedReservations = _reservationService.GetSortedReservations(_reservationsSortingType, allReservations);

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

        private Task OnSelectReservationCommandAsync(ReservationBindableModel? reservation)
        {
            SelectedReservation = reservation == SelectedReservation
                ? null
                : reservation;

            return Task.CompletedTask;
        }

        private async Task OnAddNewReservationCommandAsync()
        {
            var allAvailableTables = await GetAvailableTablesWithPopupAsync();

            if (allAvailableTables is not null)
            {
                var parameters = new DialogParameters()
                {
                    { Constants.DialogParameterKeys.TABLES, allAvailableTables },
                };

                var popupPage = new Views.Tablet.Dialogs.AddNewReservationDialog(parameters, AddNewReservationDialogCallBack);

                await PopupNavigation.PushAsync(popupPage);
            }
        }

        private async void AddNewReservationDialogCallBack(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out ReservationModel reservation))
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

        private async void CloseRemoveReservationDialogCallBack(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool accept) && accept && SelectedReservation is not null)
            {
                var reservationRemovingResult = await _reservationService.RemoveReservationByIdAsync(SelectedReservation.Id);

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

        private async void CloseInfoAboutReservationDialogCallBack(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.ACTION, out string action))
            {
                switch (action)
                {
                    case Constants.DialogParameterKeys.REMOVE:
                        await OnRemoveReservationCommandAsync();

                        break;
                    case Constants.DialogParameterKeys.ASSIGN:
                        await OnAssignReservationCommandAsync();

                        break;
                }
            }
            else
            {
                await _notificationsService.ClosePopupAsync();
            }
        }

        private async Task OnAssignReservationCommandAsync()
        {
            var allEmployees = await GetEmployeesAndShowPopupIfNeededAsync();

            if (allEmployees is not null && SelectedReservation is not null)
            {
                var allAvailableTables = await GetAvailableTablesWithPopupAsync();

                if (allAvailableTables is not null)
                {
                    if (SelectedReservation.Employee is not null)
                    {
                        var employeeId = SelectedReservation.Employee.EmployeeId;

                        var employee = allEmployees.FirstOrDefault(row => row.EmployeeId == employeeId);

                        if (employee is not null)
                        {
                            SelectedReservation.Employee = employee;
                        }
                        else
                        {
                            var employees = allEmployees.ToList();
                            employees.Add(SelectedReservation.Employee);

                            allEmployees = employees;
                        }
                    }

                    if (SelectedReservation.Table is not null)
                    {
                        var tableId = SelectedReservation.Table.Id;
                        var table = allAvailableTables.FirstOrDefault(row => row.Id == tableId);

                        if (table is not null)
                        {
                            SelectedReservation.Table = table;
                        }
                        else
                        {
                            var listTables = allAvailableTables.ToList();
                            listTables.Add(SelectedReservation.Table);

                            allAvailableTables = listTables;
                        }
                    }

                    var parameters = new DialogParameters()
                    {
                        { Constants.DialogParameterKeys.EMPLOYEE, SelectedReservation.Employee },
                        { Constants.DialogParameterKeys.TABLE, SelectedReservation.Table },
                        { Constants.DialogParameterKeys.EMPLOYEES, allEmployees },
                        { Constants.DialogParameterKeys.TABLES, allAvailableTables },
                    };

                    var popupPage = new Views.Tablet.Dialogs.AssignReservationDialog(parameters, CloseAssignReservationDialogCallBack);

                    await PopupNavigation.PushAsync(popupPage);
                }
            }
        }

        private async void CloseAssignReservationDialogCallBack(IDialogParameters parameters)
        {
            if (parameters.Count > 0)
            {
                if (parameters.TryGetValue(Constants.DialogParameterKeys.TABLE, out TableModelDTO table)
                    && parameters.TryGetValue(Constants.DialogParameterKeys.EMPLOYEE, out EmployeeModelDTO employee)
                    && SelectedReservation is not null)
                {
                    SelectedReservation.Table = table;
                    SelectedReservation.Employee = employee;

                    var reservationForUpdate = _mapper.Map<ReservationModel>(SelectedReservation);

                    var updateResult = await _reservationService.UpdateReservationAsync(reservationForUpdate);

                    if (updateResult.IsSuccess)
                    {
                        await _notificationsService.ClosePopupAsync();

                        IsReservationsRefreshing = true;
                    }
                    else
                    {
                        await ResponseToUnsuccessfulRequestAsync(updateResult.Exception?.Message);
                    }
                }
            }
            else
            {
                await _notificationsService.ClosePopupAsync();
            }
        }

        private async Task<IEnumerable<TableModelDTO>?> GetAvailableTablesWithPopupAsync()
        {
            var allAvailableTables = Enumerable.Empty<TableModelDTO>();

            var resultOfGettingAvailableTable = await _orderService.GetFreeTablesAsync();

            if (resultOfGettingAvailableTable.IsSuccess)
            {
                allAvailableTables = resultOfGettingAvailableTable.Result;

                if (allAvailableTables is null)
                {
                    await _notificationsService.ShowInfoDialogAsync(
                        LocalizationResourceManager.Current["Error"],
                        LocalizationResourceManager.Current["NoTablesAvailable"],
                        LocalizationResourceManager.Current["Ok"]);
                }
            }
            else
            {
                await ResponseToUnsuccessfulRequestAsync(resultOfGettingAvailableTable.Exception?.Message);
            }

            return allAvailableTables;
        }

        private async Task<IEnumerable<EmployeeModelDTO>?> GetEmployeesAndShowPopupIfNeededAsync()
        {
            var allEmployees = Enumerable.Empty<EmployeeModelDTO>();

            var resultOfGettingEmployees = await _employeesService.GetEmployeesAsync();

            if (resultOfGettingEmployees.IsSuccess)
            {
                allEmployees = resultOfGettingEmployees.Result;
            }
            else
            {
                await ResponseToUnsuccessfulRequestAsync(resultOfGettingEmployees.Exception?.Message);
            }

            return allEmployees;
        }

        #endregion
    }
}

using AutoMapper;
using Next2.Enums;
using Next2.Models.Bindables;
using Next2.Services.Authentication;
using Next2.Services.DishesHolding;
using Next2.Services.Menu;
using Next2.Services.Notifications;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class HoldDishesViewModel : BaseViewModel
    {
        private readonly IDishesHoldingService _dishesHolding;
        private readonly IMenuService _menuService;
        private readonly IMapper _mapper;

        private EHoldDishesSortingType _holdDishesSortingType;
        public HoldDishesViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService,
            IDishesHoldingService dishesHolding,
            IMenuService menuService,
            IMapper mapper)
            : base(navigationService, authenticationService, notificationsService)
        {
            _dishesHolding = dishesHolding;
            _mapper = mapper;
            _menuService = menuService;
        }

        #region -- Public properties --

        public bool IsHoldDishesRefreshing { get; set; }

        public bool IsNothingFound { get; set; }

        public int IndexLastVisibleElement { get; set; }

        public int TableNumber { get; set; }

        public int CurrentTableNumber { get; set; }

        public ObservableCollection<HoldDishBindableModel> HoldDishes { get; set; } = new();

        public ObservableCollection<object>? SelectedHoldDishes { get; set; }

        public ObservableCollection<TableBindableModel> Tables { get; set; } = new();

        public TableBindableModel? SelectedTable { get; set; }

        private ICommand? _refreshHoldDishesCommand;
        public ICommand RefreshHoldDishesCommand => _refreshHoldDishesCommand ??= new AsyncCommand(OnRefreshHoldDishesCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _changeSortHoldDishesCommand;
        public ICommand ChangeSortHoldDishesCommand => _changeSortHoldDishesCommand ??= new AsyncCommand<EHoldDishesSortingType>(OnChangeSortHoldDishesCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _setSelectedHoldDishesCommand;
        public ICommand SetSelectedHoldDishesCommand => _setSelectedHoldDishesCommand ??= new AsyncCommand<List<object>>(OnSetSelectedHoldDishesCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _setHoldDishesByTableNumberCommand;
        public ICommand SetHoldDishesByTableNumberCommand => _setHoldDishesByTableNumberCommand ??= new AsyncCommand(OnSetHoldDishesByTableNumberCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _selectAllHoldDishesTableCommand;
        public ICommand SelectAllHoldDishesTableCommand => _selectAllHoldDishesTableCommand ??= new AsyncCommand(OnSelectAllHoldDishesTableCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _releaseSelectionHoldDishesCommand;
        public ICommand ReleaseSelectionHoldDishesCommand => _releaseSelectionHoldDishesCommand ??= new AsyncCommand(OnReleaseSelectionHoldDishesCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            base.OnAppearing();

            await OnRefreshHoldDishesCommandAsync();
        }

        #endregion

        #region -- Private helpers --

        private async Task OnRefreshHoldDishesCommandAsync()
        {
            IsHoldDishesRefreshing = true;

            _holdDishesSortingType = EHoldDishesSortingType.ByTableNumber;
            HoldDishes = await GetAllHoldDishesAsync();

            Tables = GetHoldTablesFromHoldDishes(HoldDishes);

            IsHoldDishesRefreshing = false;
        }

        private ObservableCollection<HoldDishBindableModel> GetHoldDishesByTableNumber(int tableNumber)
        {
            if (HoldDishes.Any(x => x.TableNumber != tableNumber))
            {
                SelectedHoldDishes = null;
            }

            var holdDishes = _dishesHolding.GetHoldDishesByTableNumber(tableNumber);

            IsNothingFound = !holdDishes.Any();

            return _mapper.Map<ObservableCollection<HoldDishBindableModel>>(holdDishes);
        }

        private Task OnChangeSortHoldDishesCommandAsync(EHoldDishesSortingType holdDishesSortingType)
        {
            if (_holdDishesSortingType == holdDishesSortingType)
            {
                HoldDishes = new(HoldDishes.Reverse());
            }
            else
            {
                _holdDishesSortingType = holdDishesSortingType;

                var sortedHoldDishes = _dishesHolding.GetSortedHoldDishes(_holdDishesSortingType, HoldDishes);

                HoldDishes = new(sortedHoldDishes);
            }

            return Task.CompletedTask;
        }

        private Task OnSelectAllHoldDishesTableCommandAsync()
        {
            if (SelectedTable?.TableNumber != Constants.Limits.ALL_TABLES)
            {
                SelectedHoldDishes = SelectedHoldDishes?.Count != HoldDishes.Count
                    ? new(HoldDishes)
                    : null;
            }

            return Task.CompletedTask;
        }

        private Task OnSetSelectedHoldDishesCommandAsync(List<object>? selectedDishes)
        {
            var selectedCount = selectedDishes?.Count ?? 0;

            if (selectedCount == 0)
            {
                SelectedHoldDishes = null;
                CurrentTableNumber = 0;
            }
            else
            {
                var selectedBindableDishes = selectedDishes.Select(x => x as HoldDishBindableModel).ToList();
                var firstDish = selectedBindableDishes.FirstOrDefault();
                CurrentTableNumber = firstDish.TableNumber;
                var indexDish = selectedBindableDishes.IndexOf(selectedBindableDishes.FirstOrDefault(x => x.TableNumber != CurrentTableNumber));

                if (indexDish != -1)
                {
                    selectedDishes.RemoveAt(indexDish);
                    selectedCount--;
                }

                if (SelectedHoldDishes?.Count != selectedCount)
                {
                    SelectedHoldDishes = new(selectedDishes);
                }
            }

            return Task.CompletedTask;
        }

        private Task OnSetHoldDishesByTableNumberCommandAsync()
        {
            if (SelectedTable is not null)
            {
                TableNumber = SelectedTable.TableNumber;
                HoldDishes = GetHoldDishesByTableNumber(TableNumber);
            }

            return Task.CompletedTask;
        }

        private async Task OnReleaseSelectionHoldDishesCommandAsync()
        {
            var selectedDishes = SelectedHoldDishes.Select(x => x as HoldDishBindableModel).ToList();

            var param = new DialogParameters();

            if (selectedDishes.Count > 1)
            {
                param.Add(Constants.DialogParameterKeys.HOLD_DISHES, selectedDishes);
            }
            else
            {
                var dishId = selectedDishes.FirstOrDefault().DishId;

                var selectedDish = await GetDishByIdAsync(dishId);

                param.Add(Constants.DialogParameterKeys.DISH, selectedDish);
            }

            PopupPage holdDishDialog = App.IsTablet
                ? new Views.Tablet.Dialogs.HoldDishDialog(param, CloseHoldDishDialogCallbackAsync)
                : new Views.Mobile.Dialogs.HoldDishDialog(param, CloseHoldDishDialogCallbackAsync);

            await PopupNavigation.PushAsync(holdDishDialog);
        }

        private async void CloseHoldDishDialogCallbackAsync(IDialogParameters parameters)
        {
            await _notificationsService.CloseAllPopupAsync();

            if (IsInternetConnected)
            {
                //if (parameters.TryGetValue(Constants.DialogParameterKeys.HOLD, out DateTime holdTime))
                //{
                //    if (parameters.TryGetValue(Constants.DialogParameterKeys.SEATS, out int seatNumber))
                //    {
                //        var seats = seatNumber == Constants.Limits.ALL_SEATS
                //            ? CurrentOrder.Seats
                //            : CurrentOrder.Seats?.Where(x => x.SeatNumber == seatNumber);

                //        if (seats is not null)
                //        {
                //            foreach (var seat in seats)
                //            {
                //                foreach (var dish in seat.SelectedDishes)
                //                {
                //                    dish.HoldTime = holdTime;
                //                    isCanUpdateOrder = true;
                //                }
                //            }
                //        }
                //    }
                //    else if (SelectedDish is not null)
                //    {
                //        SelectedDish.HoldTime = holdTime;
                //        TimerHoldSelectedDish = holdTime.AddMinutes(1) - DateTime.Now;
                //        isCanUpdateOrder = true;
                //    }
                //}

                //if (isCanUpdateOrder)
                //{
                //    var resultOfUpdatingOrder = await _orderService.UpdateCurrentOrderAsync();

                //    if (!resultOfUpdatingOrder.IsSuccess)
                //    {
                //        await ResponseToUnsuccessfulRequestAsync(resultOfUpdatingOrder.Exception?.Message);
                //    }
                //}
            }
            else
            {
                await _notificationsService.ShowNoInternetConnectionDialogAsync();
            }
        }

        private ObservableCollection<TableBindableModel> GetHoldTablesFromHoldDishes(ObservableCollection<HoldDishBindableModel> holdDishes)
        {
            var bindableTables = new ObservableCollection<TableBindableModel>();

            if (holdDishes.Any())
            {
                var tables = holdDishes.GroupBy(x => x.TableNumber).Select(y => y.First());

                bindableTables = _mapper.Map<ObservableCollection<TableBindableModel>>(tables);

                bindableTables.Add(new TableBindableModel { TableNumber = Constants.Limits.ALL_TABLES, });

                bindableTables = new(bindableTables.OrderBy(x => x.TableNumber));
                SelectedTable = bindableTables.FirstOrDefault();
            }

            return bindableTables;
        }

        private async Task<ObservableCollection<HoldDishBindableModel>> GetAllHoldDishesAsync()
        {
            var holdDishes = new ObservableCollection<HoldDishBindableModel>();

            var resultOfGettingHoldDishes = await _dishesHolding.GetHoldDishesAsync();

            if (resultOfGettingHoldDishes.IsSuccess)
            {
                holdDishes = _mapper.Map<ObservableCollection<HoldDishBindableModel>>(resultOfGettingHoldDishes.Result);
            }
            else
            {
                await ResponseToUnsuccessfulRequestAsync(resultOfGettingHoldDishes.Exception?.Message);
            }

            IsNothingFound = !holdDishes.Any();

            return holdDishes;
        }

        private async Task<DishBindableModel> GetDishByIdAsync(Guid dishId)
        {
            var dish = new DishBindableModel();

            var resultOfGettingDish = await _menuService.GetDishByIdAsync(dishId);

            if (resultOfGettingDish.IsSuccess)
            {
                dish = _mapper.Map<DishBindableModel>(resultOfGettingDish.Result);
            }
            else
            {
                await ResponseToUnsuccessfulRequestAsync(resultOfGettingDish.Exception?.Message);
            }

            return dish;
        }

        #endregion
    }
}

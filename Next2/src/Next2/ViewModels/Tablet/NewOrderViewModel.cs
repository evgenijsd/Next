using Acr.UserDialogs;
using AutoMapper;
using Next2.Enums;
using Next2.Interfaces;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Menu;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Next2.Services.WorkLog;
using Next2.Views.Tablet;
using Prism.Navigation;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet
{
    public class NewOrderViewModel : BaseViewModel, IPageActionsHandler
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        private readonly IWorkLogService _workLogService;
        private readonly IMapper _mapper;
        private readonly INotificationsService _notificationsService;

        private FullOrderBindableModel _tempCurrentOrder;
        private SeatBindableModel _tempCurrentSeat;

        private bool _shouldOrderDishesByDESC;

        public NewOrderViewModel(
            INavigationService navigationService,
            IMenuService menuService,
            OrderRegistrationViewModel orderRegistrationViewModel,
            IWorkLogService workLogService,
            IOrderService orderService,
            INotificationsService notificationsService,
            IMapper mapper)
            : base(navigationService)
        {
            _menuService = menuService;
            _orderService = orderService;
            _workLogService = workLogService;
            _mapper = mapper;
            _notificationsService = notificationsService;

            OrderRegistrationViewModel = orderRegistrationViewModel;

            orderRegistrationViewModel?.RefreshCurrentOrderAsync();
        }

        #region -- Public properties --

        public ELoadingState DishesLoadingState { get; set; } = ELoadingState.InProgress;

        public ELoadingState CategoriesLoadingState { get; set; } = ELoadingState.InProgress;

        public DateTime CurrentDateTime { get; set; } = DateTime.Now;

        public ObservableCollection<CategoryModel> Categories { get; set; }

        public CategoryModel? SelectedCategoriesItem { get; set; }

        public ObservableCollection<DishModelDTO> Dishes { get; set; }

        public ObservableCollection<SubcategoryModel> Subcategories { get; set; }

        public OrderRegistrationViewModel OrderRegistrationViewModel { get; set; }

        public SubcategoryModel? SelectedSubcategoriesItem { get; set; }

        private ICommand? _tapDishCommand;
        public ICommand TapDishCommand => _tapDishCommand ??= new AsyncCommand<DishModelDTO>(OnTapDishCommand, allowsMultipleExecutions: false);

        private ICommand? _tapSortCommand;
        public ICommand TapSortCommand => _tapSortCommand ??= new AsyncCommand(OnTapSortCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _tapExpandCommand;
        public ICommand TapExpandCommand => _tapExpandCommand ??= new AsyncCommand(OnTapExpandCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _openEmployeeWorkingHoursCommand;
        public ICommand OpenEmployeeWorkingHoursCommand => _openEmployeeWorkingHoursCommand ??= new AsyncCommand(OnOpenEmployeeWorkingHoursCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _refreshDishesCommand;
        public ICommand RefreshDishesCommand => _refreshDishesCommand ??= new AsyncCommand(OnRefreshDishesCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _refreshCategoriesCommand;
        public ICommand RefreshCategoriesCommand => _refreshCategoriesCommand ??= new AsyncCommand(OnRefreshCategoriesCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            base.OnAppearing();

            _shouldOrderDishesByDESC = false;

            await OrderRegistrationViewModel.InitializeAsync(null);

            await OnRefreshCategoriesCommandAsync();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            SelectedSubcategoriesItem = null;
            SelectedCategoriesItem = null;

            Dishes = new();
            Subcategories = new();
            Categories = new();

            OrderRegistrationViewModel.CurrentState = ENewOrderViewState.InProgress;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedCategoriesItem):
                    Task.Run(LoadSubcategoriesAsync);
                    break;
                case nameof(SelectedSubcategoriesItem):
                    Task.Run(OnRefreshDishesCommandAsync);
                    break;
            }
        }

        #endregion

        #region -- Private methods --

        private Task OnTapSortCommandAsync()
        {
            _shouldOrderDishesByDESC = !_shouldOrderDishesByDESC;
            Dishes = new(Dishes.Reverse());

            return Task.CompletedTask;
        }

        private Task OnTapDishCommand(DishModelDTO dish)
        {
            var param = new DialogParameters
            {
                { Constants.DialogParameterKeys.DISH, dish },
                { Constants.DialogParameterKeys.DISCOUNT_PRICE, _orderService.CurrentOrder.DiscountPrice },
            };

            return PopupNavigation.PushAsync(new Views.Tablet.Dialogs.AddDishToOrderDialog(param, CloseDialogCallback));
        }

        private async void CloseDialogCallback(IDialogParameters dialogResult)
        {
            if (IsInternetConnected)
            {
                if (dialogResult.ContainsKey(Constants.DialogParameterKeys.DISH))
                {
                    if (dialogResult.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel dish))
                    {
                        _tempCurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);
                        _tempCurrentSeat = _mapper.Map<SeatBindableModel>(_orderService.CurrentSeat);

                        var resultOfAddingDish = await _orderService.AddDishInCurrentOrderAsync(dish);

                        if (resultOfAddingDish.IsSuccess)
                        {
                            var resultOfUpdatingOrder = await _orderService.UpdateCurrentOrderAsync();

                            if (resultOfUpdatingOrder.IsSuccess)
                            {
                                await _notificationsService.CloseAllPopupAsync();

                                var toastConfig = new ToastConfig(LocalizationResourceManager.Current["SuccessfullyAddedToOrder"])
                                {
                                    Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
                                    Position = ToastPosition.Bottom,
                                };

                                UserDialogs.Instance.Toast(toastConfig);

                                await OrderRegistrationViewModel.RefreshCurrentOrderAsync();
                            }
                            else
                            {
                                await _notificationsService.CloseAllPopupAsync();

                                _orderService.CurrentOrder = _tempCurrentOrder;
                                _orderService.CurrentSeat = _tempCurrentSeat;

                                await OrderRegistrationViewModel.RefreshCurrentOrderAsync();
                                await _notificationsService.ResponseToBadRequestAsync(resultOfUpdatingOrder.Exception.Message);
                            }
                        }
                        else
                        {
                            await _notificationsService.CloseAllPopupAsync();

                            await _notificationsService.ShowInfoDialogAsync(
                                LocalizationResourceManager.Current["Error"],
                                LocalizationResourceManager.Current["SomethingWentWrong"],
                                LocalizationResourceManager.Current["Ok"]);
                        }
                    }
                }
                else
                {
                    await _notificationsService.CloseAllPopupAsync();
                }
            }
            else
            {
                await _notificationsService.CloseAllPopupAsync();

                await _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        private async Task OnRefreshCategoriesCommandAsync()
        {
            OrderRegistrationViewModel.CurrentState = ENewOrderViewState.InProgress;

            CategoriesLoadingState = ELoadingState.InProgress;

            if (IsInternetConnected)
            {
                var resultGettingCategories = await _menuService.GetAllCategoriesAsync();

                if (resultGettingCategories.IsSuccess)
                {
                    Categories = new(resultGettingCategories.Result);
                    SelectedCategoriesItem = Categories.FirstOrDefault();

                    CategoriesLoadingState = ELoadingState.Completed;
                    OrderRegistrationViewModel.CurrentState = ENewOrderViewState.Default;
                }
                else
                {
                    CategoriesLoadingState = ELoadingState.Error;
                }
            }
            else
            {
                CategoriesLoadingState = ELoadingState.NoInternet;
            }
        }

        private Task LoadSubcategoriesAsync()
        {
            if (SelectedCategoriesItem is not null)
            {
                Subcategories = new(SelectedCategoriesItem.Subcategories);

                Subcategories.Insert(0, new SubcategoryModel()
                {
                    Id = Guid.Empty,
                    Name = LocalizationResourceManager.Current["All"],
                });

                SelectedSubcategoriesItem = Subcategories.FirstOrDefault();
            }

            return Task.CompletedTask;
        }

        private async Task OnRefreshDishesCommandAsync()
        {
            DishesLoadingState = ELoadingState.InProgress;

            if (IsInternetConnected && SelectedCategoriesItem is not null && SelectedSubcategoriesItem is not null)
            {
                var resultGettingDishes = await _menuService.GetDishesAsync(SelectedCategoriesItem.Id, SelectedSubcategoriesItem.Id);

                if (resultGettingDishes.IsSuccess)
                {
                    Dishes = _shouldOrderDishesByDESC
                        ? new(resultGettingDishes.Result.OrderByDescending(row => row.Name))
                        : new(resultGettingDishes.Result.OrderBy(row => row.Name));

                    DishesLoadingState = ELoadingState.Completed;
                }
                else
                {
                    DishesLoadingState = ELoadingState.Error;
                }
            }
            else
            {
                DishesLoadingState = ELoadingState.NoInternet;
            }
        }

        private Task OnTapExpandCommandAsync()
        {
            return IsInternetConnected
                ? _navigationService.NavigateAsync(nameof(ExpandPage))
                : _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
        }

        private Task OnOpenEmployeeWorkingHoursCommandAsync()
        {
            var popupPage = new Views.Tablet.Dialogs.EmployeeTimeClockDialog(
                _workLogService,
                (IDialogParameters dialogResult) => PopupNavigation.PopAsync());

            return PopupNavigation.PushAsync(popupPage);
        }

        #endregion
    }
}
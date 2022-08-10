using Acr.UserDialogs;
using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Menu;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class ChooseDishPageViewModel : BaseViewModel
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly INotificationsService _notificationsService;

        private FullOrderBindableModel _tempCurrentOrder = new();
        private SeatBindableModel _tempCurrentSeat = new();

        private bool _shouldOrderDishesByDESC;

        public ChooseDishPageViewModel(
            IMenuService menuService,
            INavigationService navigationService,
            IOrderService orderService,
            INotificationsService notificationsService,
            IMapper mapper)
            : base(navigationService)
        {
            _menuService = menuService;
            _orderService = orderService;
            _mapper = mapper;
            _notificationsService = notificationsService;
        }

        #region -- Public properties --

        public ELoadingState DishesLoadingState { get; set; } = ELoadingState.InProgress;

        public CategoryModel SelectedCategoriesItem { get; set; }

        public ObservableCollection<DishModelDTO> Dishes { get; set; } = new();

        public ObservableCollection<SubcategoryModel> Subcategories { get; set; } = new();

        public SubcategoryModel SelectedSubcategoriesItem { get; set; }

        private ICommand? _tapDishCommand;
        public ICommand TapDishCommand => _tapDishCommand ??= new AsyncCommand<DishModelDTO>(OnTapDishCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _tapSortCommand;
        public ICommand TapSortCommand => _tapSortCommand ??= new AsyncCommand(OnTapSortCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _refreshDishesCommand;
        public ICommand RefreshDishesCommand => _refreshDishesCommand ??= new AsyncCommand(OnRefreshDishesCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override Task InitializeAsync(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.CATEGORY, out CategoryModel category))
            {
                SelectedCategoriesItem = category;
            }

            return Task.CompletedTask;
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

        #region -- Private helpers --

        private Task OnTapSortCommandAsync()
        {
            _shouldOrderDishesByDESC = !_shouldOrderDishesByDESC;
            Dishes = new(Dishes.Reverse());

            return Task.CompletedTask;
        }

        private Task OnTapDishCommandAsync(DishModelDTO? dish)
        {
            var param = new DialogParameters
            {
                { Constants.DialogParameterKeys.DISH, dish },
                { Constants.DialogParameterKeys.DISCOUNT_PRICE, _orderService.CurrentOrder.DiscountPrice },
            };

            return PopupNavigation.PushAsync(new Views.Mobile.Dialogs.AddDishToOrderDialog(param, CloseDialogCallback));
        }

        private async void CloseDialogCallback(IDialogParameters dialogResult)
        {
            if (IsInternetConnected)
            {
                if (dialogResult.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel dish))
                {
                    _tempCurrentOrder = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);
                    _tempCurrentSeat = _mapper.Map<SeatBindableModel>(_orderService.CurrentSeat);

                    var resultOfAddingDishToOrder = await _orderService.AddDishInCurrentOrderAsync(dish);

                    if (resultOfAddingDishToOrder.IsSuccess)
                    {
                        var resultOfUpdatingOrder = await _orderService.UpdateCurrentOrderAsync();

                        await _notificationsService.CloseAllPopupAsync();

                        if (resultOfUpdatingOrder.IsSuccess)
                        {
                            var toastConfig = new ToastConfig(LocalizationResourceManager.Current["SuccessfullyAddedToOrder"])
                            {
                                Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
                                Position = ToastPosition.Bottom,
                            };

                            UserDialogs.Instance.Toast(toastConfig);
                        }
                        else
                        {
                            _orderService.CurrentOrder = _tempCurrentOrder;
                            _orderService.CurrentSeat = _tempCurrentSeat;
                            _orderService.UpdateTotalSum(_orderService.CurrentOrder);

                            await _notificationsService.ResponseToBadRequestAsync(resultOfUpdatingOrder.Exception?.Message);
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

        private async Task OnRefreshDishesCommandAsync()
        {
            DishesLoadingState = ELoadingState.InProgress;

            if (IsInternetConnected)
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

        #endregion
    }
}
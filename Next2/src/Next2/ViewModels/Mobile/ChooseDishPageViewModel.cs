using Acr.UserDialogs;
using Next2.Enums;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Menu;
using Next2.Services.Order;
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

namespace Next2.ViewModels.Mobile
{
    public class ChooseDishPageViewModel : BaseViewModel
    {
        private readonly IMenuService _menuService;

        private readonly IOrderService _orderService;

        private bool _shouldOrderDishesByDESC;

        public ChooseDishPageViewModel(
            IMenuService menuService,
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _menuService = menuService;
            _orderService = orderService;
        }

        #region -- Public properties --

        public CategoryModel SelectedCategoriesItem { get; set; }

        public ObservableCollection<DishModelDTO> Dishes { get; set; }

        public ObservableCollection<SubcategoryModel> Subcategories { get; set; }

        public SubcategoryModel SelectedSubcategoriesItem { get; set; }

        private ICommand _tapDishCommand;
        public ICommand TapDishCommand => _tapDishCommand ??= new AsyncCommand<DishModelDTO>(OnTapDishCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapSortCommand;
        public ICommand TapSortCommand => _tapSortCommand ??= new AsyncCommand(OnTapSortCommandAsync, allowsMultipleExecutions: false);

        private ICommand _refreshDishesCommand;
        public ICommand RefreshDishesCommand => _refreshDishesCommand ??= new AsyncCommand(OnLoadDishesCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.CATEGORY, out CategoryModel category))
            {
                SelectedCategoriesItem = category;
            }
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
                    Task.Run(OnLoadDishesCommandAsync);
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

        private Task OnTapDishCommandAsync(DishModelDTO dish)
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
            if (dialogResult is not null && dialogResult.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel dish))
            {
                var resultOfAddingDishToCurrentOrder = await _orderService.AddDishInCurrentOrderAsync(dish);

                if (resultOfAddingDishToCurrentOrder.IsSuccess)
                {
                    await _orderService.UpdateCurrentOrderAsync();

                    if (PopupNavigation.PopupStack.Any())
                    {
                        await PopupNavigation.PopAsync();
                    }

                    var toastConfig = new ToastConfig(LocalizationResourceManager.Current["SuccessfullyAddedToOrder"])
                    {
                        Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
                        Position = ToastPosition.Bottom,
                    };

                    UserDialogs.Instance.Toast(toastConfig);
                }
            }
            else
            {
                await PopupNavigation.PopAsync();
            }
        }

        private async Task OnLoadDishesCommandAsync()
        {
            DataLoadingState = EStateLoad.Loading;

            if (IsInternetConnected)
            {
                var resultGettingDishes = await _menuService.GetDishesAsync(SelectedCategoriesItem.Id, SelectedSubcategoriesItem.Id);

                if (resultGettingDishes.IsSuccess)
                {
                    Dishes = _shouldOrderDishesByDESC
                        ? new(resultGettingDishes.Result.OrderByDescending(row => row.Name))
                        : new(resultGettingDishes.Result.OrderBy(row => row.Name));

                    DataLoadingState = EStateLoad.Loaded;
                }
                else
                {
                    DataLoadingState = EStateLoad.Error;
                }
            }
            else
            {
                DataLoadingState = EStateLoad.NoInternet;
            }
        }

        private Task LoadSubcategoriesAsync()
        {
            if (IsInternetConnected && SelectedCategoriesItem is not null)
            {
                Subcategories = new(SelectedCategoriesItem.Subcategories);

                Subcategories.Insert(0, new SubcategoryModel()
                {
                    Id = Guid.Empty,
                    Name = LocalizationResourceManager.Current["All"],
                });

                SelectedSubcategoriesItem = Subcategories.FirstOrDefault();
            }
            else
            {
                DataLoadingState = EStateLoad.NoInternet;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
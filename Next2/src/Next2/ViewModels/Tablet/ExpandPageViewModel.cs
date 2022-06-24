﻿using Acr.UserDialogs;
using Next2.Extensions;
using Next2.Interfaces;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Resources.Strings;
using Next2.Services.Menu;
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

namespace Next2.ViewModels.Tablet
{
    public class ExpandPageViewModel : BaseViewModel, IPageActionsHandler
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;

        private bool _shouldOrderDishesByDESC;

        public ExpandPageViewModel(
            INavigationService navigationService,
            IOrderService orderService,
            IMenuService menuService)
            : base(navigationService)
        {
            _menuService = menuService;
            _orderService = orderService;
        }

        #region -- Public properties --

        public int HeightCollectionView { get; set; }

        public ObservableCollection<CategoryModel> Categories { get; set; }

        public CategoryModel? SelectedCategoriesItem { get; set; }

        public ObservableCollection<DishModelDTO> Dishes { get; set; }

        public ObservableCollection<SubcategoryModel> Subcategories { get; set; }

        public SubcategoryModel? SelectedSubcategoriesItem { get; set; }

        private ICommand _tapDishCommand;
        public ICommand TapDishCommand => _tapDishCommand ??= new AsyncCommand<DishModelDTO>(OnTapDishCommand, allowsMultipleExecutions: false);

        private ICommand _tapSortCommand;
        public ICommand TapSortCommand => _tapSortCommand ??= new AsyncCommand(OnTapSortCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            _shouldOrderDishesByDESC = false;
            Task.Run(LoadCategoriesAsync);
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
                    Task.Run(LoadDishesAsync);
                    break;
            }
        }

        #endregion

        #region -- Private methods --

        private async Task OnTapSortCommandAsync()
        {
            _shouldOrderDishesByDESC = !_shouldOrderDishesByDESC;
            Dishes = new(Dishes.Reverse());
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
            if (dialogResult is not null && dialogResult.ContainsKey(Constants.DialogParameterKeys.DISH))
            {
                if (dialogResult.TryGetValue(Constants.DialogParameterKeys.DISH, out DishBindableModel dish))
                {
                    var result = await _orderService.AddDishInCurrentOrderAsync(dish);

                    if (result.IsSuccess)
                    {
                        await _orderService.UpdateOrderAsync(_orderService.CurrentOrder.ToUpdateOrderCommand());

                        await PopupNavigation.PopAsync();

                        var toastConfig = new ToastConfig(LocalizationResourceManager.Current["SuccessfullyAddedToOrder"])
                        {
                            Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
                            Position = ToastPosition.Bottom,
                        };

                        UserDialogs.Instance.Toast(toastConfig);
                    }
                }
            }
            else
            {
                await PopupNavigation.PopAsync();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            if (IsInternetConnected)
            {
                var resultGettingCategories = await _menuService.GetAllCategoriesAsync();

                if (resultGettingCategories.IsSuccess)
                {
                    Categories = new(resultGettingCategories.Result);
                    SelectedCategoriesItem = Categories.FirstOrDefault();

                    HeightCollectionView = (int)((Math.Ceiling((double)Categories.Count / 7) * (54 + 10)) - 8);
                }
            }
        }

        private async Task LoadDishesAsync()
        {
            if (IsInternetConnected && SelectedCategoriesItem is not null && SelectedSubcategoriesItem is not null)
            {
                var resultGettingDishes = await _menuService.GetDishesAsync(SelectedCategoriesItem.Id, SelectedSubcategoriesItem.Id);

                if (resultGettingDishes.IsSuccess)
                {
                    Dishes = _shouldOrderDishesByDESC
                        ? new(resultGettingDishes.Result.OrderByDescending(row => row.Name))
                        : new(resultGettingDishes.Result.OrderBy(row => row.Name));
                }
            }
        }

        private async Task LoadSubcategoriesAsync()
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
        }

        #endregion
    }
}
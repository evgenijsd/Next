using Acr.UserDialogs;
using Next2.Helpers.DTO.Categories;
using Next2.Interfaces;
using Next2.Models.Api;
using Next2.Resources.Strings;
using Next2.Services.Menu;
using Next2.Services.Order;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Tablet
{
    public class ExpandPageViewModel : BaseViewModel, IPageActionsHandler
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        private readonly IPopupNavigation _popupNavigation;

        private bool _order;

        public ExpandPageViewModel(
            INavigationService navigationService,
            IOrderService orderService,
            IMenuService menuService,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _menuService = menuService;
            _orderService = orderService;
            _popupNavigation = popupNavigation;

            Task.Run(LoadCategoriesAsync);
        }

        #region -- Public properties --

        public int HeightCollectionView { get; set; }

        public ObservableCollection<CategoryModel> CategoriesItems { get; set; }

        public CategoryModel SelectedCategoriesItem { get; set; }

        //public ObservableCollection<SetModel> SetsItems { get; set; }
        public ObservableCollection<SubcategoryModel> SubcategoriesItems { get; set; }

        public SubcategoryModel SelectedSubcategoriesItem { get; set; }

        //private ICommand _tapSetCommand;
        //public ICommand TapSetCommand => _tapSetCommand ??= new AsyncCommand<SetModel>(OnTapSetCommandAsync, allowsMultipleExecutions: false);
        private ICommand _tapSortCommand;
        public ICommand TapSortCommand => _tapSortCommand ??= new AsyncCommand(OnTapSortCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            _order = false;
            Task.Run(LoadCategoriesAsync);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            SelectedCategoriesItem = new();
            SelectedSubcategoriesItem = new();
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedCategoriesItem):
                    Task.Run(LoadSubcategoriesAsync);
                    break;
                //case nameof(SelectedSubcategoriesItem):
                //    Task.Run(LoadSetsAsync);
                //    break;
            }
        }

        #endregion

        #region -- Private methods --

        private async Task OnTapSortCommandAsync()
        {
            _order = !_order;
            //await LoadSetsAsync();
        }

        //private async Task OnTapSetCommandAsync(SetModel set)
        //{
        //    var portions = await _menuService.GetPortionsSetAsync(set.Id);

        //    if (portions.IsSuccess)
        //    {
        //        var param = new DialogParameters
        //        {
        //            { Constants.DialogParameterKeys.SET, set },
        //            { Constants.DialogParameterKeys.PORTIONS, portions.Result },
        //        };

        //        await _popupNavigation.PushAsync(new Views.Tablet.Dialogs.AddSetToOrderDialog(param, CloseDialogCallback));
        //    }
        //}
        //private async void CloseDialogCallback(IDialogParameters dialogResult)
        //{
        //    if (dialogResult is not null && dialogResult.ContainsKey(Constants.DialogParameterKeys.SET))
        //    {
        //        if (dialogResult.TryGetValue(Constants.DialogParameterKeys.SET, out SetBindableModel set))
        //        {
        //            var result = await _orderService.AddSetInCurrentOrderAsync(set);

        //            if (result.IsSuccess)
        //            {
        //                await _popupNavigation.PopAsync();

        //                var toastConfig = new ToastConfig(Strings.SuccessfullyAddedToOrder)
        //                {
        //                    Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
        //                    Position = ToastPosition.Bottom,
        //                };

        //                UserDialogs.Instance.Toast(toastConfig);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        await _popupNavigation.PopAsync();
        //    }
        //}
        private async Task LoadCategoriesAsync()
        {
            if (IsInternetConnected)
            {
                var resultCategories = await _menuService.GetAllCategoriesAsync();

                if (resultCategories.IsSuccess)
                {
                    var categories = resultCategories.Result.Select(row => new CategoryModel()
                    {
                        Id = row.Id,
                        Name = row.Name,
                        Subcategories = new(row.Subcategories.Select(row => new SubcategoryModel()
                        {
                            Id = row.Id,
                            Name = row.Name
                        })),
                    });

                    CategoriesItems = new(categories);
                    SelectedCategoriesItem = CategoriesItems.FirstOrDefault();

                    HeightCollectionView = (int)((Math.Ceiling((double)CategoriesItems.Count / 7) * (54 + 10)) - 8);
                }
            }
        }

        //private async Task LoadSetsAsync()
        //{
        //    if (IsInternetConnected)
        //    {
        //        var resultSets = await _menuService.GetSetsAsync(SelectedCategoriesItem.Id, SelectedSubcategoriesItem.Id);

        //        if (resultSets.IsSuccess)
        //        {
        //            if (_order)
        //            {
        //                SetsItems = new(resultSets.Result.OrderByDescending(row => row.Title));
        //            }
        //            else
        //            {
        //                SetsItems = new(resultSets.Result.OrderBy(row => row.Title));
        //            }
        //        }
        //    }
        //}
        private async Task LoadSubcategoriesAsync()
        {
            if (IsInternetConnected && SelectedCategoriesItem is not null)
            {
                SubcategoriesItems = new(SelectedCategoriesItem.Subcategories);
                SubcategoriesItems.Insert(0, new SubcategoryModel()
                {
                    Name = LocalizationResourceManager.Current["All"],
                });

                SelectedSubcategoriesItem = SubcategoriesItems.FirstOrDefault();
            }
        }

        #endregion
    }
}
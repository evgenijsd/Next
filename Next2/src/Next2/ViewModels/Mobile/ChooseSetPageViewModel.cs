using Next2.Models.Api;
using Next2.Models.Api.DTO;
using Next2.Services.Menu;
using Next2.Services.Order;
using Prism.Navigation;
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
    public class ChooseSetPageViewModel : BaseViewModel
    {
        private readonly IMenuService _menuService;

        private readonly IPopupNavigation _popupNavigation;

        private readonly IOrderService _orderService;

        private bool _shouldOrderDishesByDESC;

        public ChooseSetPageViewModel(
            IMenuService menuService,
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            IOrderService orderService)
            : base(navigationService)
        {
            _menuService = menuService;
            _popupNavigation = popupNavigation;
            _orderService = orderService;
        }

        #region -- Public properties --

        public CategoryModel SelectedCategoriesItem { get; set; }

        public ObservableCollection<DishModelDTO> DishesItems { get; set; }

        public ObservableCollection<SubcategoryModel> SubcategoriesItems { get; set; }

        public SubcategoryModel SelectedSubcategoriesItem { get; set; }

        //private ICommand _tapSetCommand;
        //public ICommand TapSetCommand => _tapSetCommand ??= new AsyncCommand<SetModel>(OnTapSetCommandAsync, allowsMultipleExecutions: false);
        private ICommand _tapSortCommand;
        public ICommand TapSortCommand => _tapSortCommand ??= new AsyncCommand(OnTapSortCommandAsync, allowsMultipleExecutions: false);

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
                    Task.Run(LoadDishesAsync);
                    break;
            }
        }

        #endregion

        #region -- Private methods --

        private async Task OnTapSortCommandAsync()
        {
            _shouldOrderDishesByDESC = !_shouldOrderDishesByDESC;
            DishesItems = new(DishesItems.Reverse());
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

        //        await _popupNavigation.PushAsync(new Views.Mobile.Dialogs.AddSetToOrderDialog(param, CloseDialogCallback));
        //    }
        //}
        //private async void CloseDialogCallback(IDialogParameters dialogResult)
        //{
        //    if (dialogResult is not null && dialogResult.TryGetValue(Constants.DialogParameterKeys.SET, out SetBindableModel set))
        //    {
        //        await _orderService.AddSetInCurrentOrderAsync(set);

        //        if (_popupNavigation.PopupStack.Any())
        //        {
        //            await _popupNavigation.PopAsync();
        //        }

        //        var toastConfig = new ToastConfig(Strings.SuccessfullyAddedToOrder)
        //        {
        //            Duration = TimeSpan.FromSeconds(Constants.Limits.TOAST_DURATION),
        //            Position = ToastPosition.Bottom,
        //        };

        //        UserDialogs.Instance.Toast(toastConfig);
        //    }
        //    else
        //    {
        //        await _popupNavigation.PopAsync();
        //    }
        //}
        private async Task LoadDishesAsync()
        {
            if (IsInternetConnected)
            {
                var response = await _menuService.GetDishesAsync(SelectedCategoriesItem.Id, SelectedSubcategoriesItem.Id);

                if (response.IsSuccess)
                {
                    DishesItems = _shouldOrderDishesByDESC ?
                        new(response.Result.OrderByDescending(row => row.Name))
                        : new(response.Result.OrderBy(row => row.Name));
                }
            }
        }

        private async Task LoadSubcategoriesAsync()
        {
            if (IsInternetConnected && SelectedCategoriesItem is not null)
            {
                SubcategoriesItems = new(SelectedCategoriesItem.Subcategories);

                SubcategoriesItems.Insert(0, new SubcategoryModel()
                {
                    Id = Guid.Empty,
                    Name = LocalizationResourceManager.Current["All"],
                });

                SelectedSubcategoriesItem = SubcategoriesItems.FirstOrDefault();
            }
        }

        #endregion
    }
}
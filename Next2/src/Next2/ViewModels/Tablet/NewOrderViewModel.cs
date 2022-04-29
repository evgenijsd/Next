using Acr.UserDialogs;
using Next2.Interfaces;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Log;
using Next2.Services.Menu;
using Next2.Services.Order;
using Next2.Views.Tablet;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet
{
    public class NewOrderViewModel : BaseViewModel, IPageActionsHandler
    {
        private readonly IMenuService _menuService;
        private readonly IPopupNavigation _popupNavigation;
        private readonly IOrderService _orderService;
        private readonly ILogService _logService;

        private bool _order;

        public NewOrderViewModel(
            INavigationService navigationService,
            IMenuService menuService,
            IPopupNavigation popupNavigation,
            OrderRegistrationViewModel orderRegistrationViewModel,
            ILogService logService,
            IOrderService orderService)
            : base(navigationService)
        {
            _menuService = menuService;
            _popupNavigation = popupNavigation;
            _orderService = orderService;
            _logService = logService;
            OrderRegistrationViewModel = orderRegistrationViewModel;

            _orderService = orderService;

            orderRegistrationViewModel.RefreshCurrentOrderAsync();
        }

        #region -- Public properties --

        public DateTime CurrentDateTime { get; set; } = DateTime.Now;

        public ObservableCollection<CategoryModel> CategoriesItems { get; set; }

        public CategoryModel? SelectedCategoriesItem { get; set; }

        public ObservableCollection<SetModel> SetsItems { get; set; }

        public ObservableCollection<SubcategoryModel> SubcategoriesItems { get; set; }

        public OrderRegistrationViewModel OrderRegistrationViewModel { get; set; }

        public SubcategoryModel? SelectedSubcategoriesItem { get; set; }

        private ICommand _tapSetCommand;
        public ICommand TapSetCommand => _tapSetCommand ??= new AsyncCommand<SetModel>(OnTapSetCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapSortCommand;
        public ICommand TapSortCommand => _tapSortCommand ??= new AsyncCommand(OnTapSortCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapExpandCommand;
        public ICommand TapExpandCommand => _tapExpandCommand ??= new AsyncCommand(OnTapExpandCommandAsync, allowsMultipleExecutions: false);

        private ICommand _employeeTimeClockPopupCallCommand;
        public ICommand EmployeeTimeClockPopupCallCommand => _employeeTimeClockPopupCallCommand ??= new AsyncCommand(OnEmployeeTimeClockPopupCallCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            _order = false;
            Task.Run(LoadCategoriesAsync);

            OrderRegistrationViewModel.InitializeAsync(null);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            SelectedCategoriesItem = null;
            SelectedSubcategoriesItem = null;
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
                    Task.Run(LoadSetsAsync);
                    break;
            }
        }

        #endregion

        #region -- Private methods --

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() => { CurrentDateTime = DateTime.Now; });
        }

        private async Task OnTapSortCommandAsync()
        {
            _order = !_order;
            await LoadSetsAsync();
        }

        private async Task OnTapSetCommandAsync(SetModel set)
        {
            var portions = await _menuService.GetPortionsSetAsync(set.Id);

            if (portions.IsSuccess)
            {
                var param = new DialogParameters
                {
                    { Constants.DialogParameterKeys.SET, set },
                    { Constants.DialogParameterKeys.PORTIONS, portions.Result },
                };

                await _popupNavigation.PushAsync(new Views.Tablet.Dialogs.AddSetToOrderDialog(param, CloseDialogCallback));
            }
        }

        private async void CloseDialogCallback(IDialogParameters dialogResult)
        {
            if (dialogResult is not null && dialogResult.ContainsKey(Constants.DialogParameterKeys.SET))
            {
                if (dialogResult.TryGetValue(Constants.DialogParameterKeys.SET, out SetBindableModel set))
                {
                    var result = await _orderService.AddSetInCurrentOrderAsync(set);

                    if (result.IsSuccess)
                    {
                        if (_popupNavigation.PopupStack.Any())
                        {
                            await _popupNavigation.PopAsync();
                        }

                        OrderRegistrationViewModel.RefreshCurrentOrderAsync();

                        var toastConfig = new ToastConfig(Strings.SuccessfullyAddedToOrder)
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
                await _popupNavigation.PopAsync();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            if (IsInternetConnected)
            {
                var resultCategories = await _menuService.GetCategoriesAsync();

                if (resultCategories.IsSuccess)
                {
                    CategoriesItems = new (resultCategories.Result);
                    SelectedCategoriesItem = CategoriesItems.FirstOrDefault();
                }
            }
        }

        private async Task LoadSetsAsync()
        {
            if (IsInternetConnected && SelectedCategoriesItem is not null && SelectedSubcategoriesItem is not null)
            {
                var resultSets = await _menuService.GetSetsAsync(SelectedCategoriesItem.Id, SelectedSubcategoriesItem.Id);

                if (resultSets.IsSuccess)
                {
                    if (_order)
                    {
                        SetsItems = new (resultSets.Result.OrderByDescending(row => row.Title));
                    }
                    else
                    {
                        SetsItems = new (resultSets.Result.OrderBy(row => row.Title));
                    }
                }
            }
        }

        private async Task LoadSubcategoriesAsync()
        {
            if (IsInternetConnected && SelectedCategoriesItem is not null)
            {
                var resultSubcategories = await _menuService.GetSubcategoriesAsync(SelectedCategoriesItem.Id);

                if (resultSubcategories.IsSuccess)
                {
                    SubcategoriesItems = new (resultSubcategories.Result);
                    SubcategoriesItems.Insert(0, new SubcategoryModel()
                    {
                        Id = 0,
                        CategoryId = SelectedCategoriesItem.Id,
                        Title = "All",
                    });

                    SelectedSubcategoriesItem = SubcategoriesItems.FirstOrDefault();
                }
            }
        }

        private async Task OnTapExpandCommandAsync()
        {
            await _navigationService.NavigateAsync(nameof(ExpandPage));
        }

        private Task OnEmployeeTimeClockPopupCallCommandAsync()
        {
            return _popupNavigation
                .PushAsync(new Views.Tablet.Dialogs
                .EmployeeTimeClockDialog(_logService, (IDialogParameters dialogResult) => _popupNavigation.PopAsync()));
        }

        #endregion
    }
}
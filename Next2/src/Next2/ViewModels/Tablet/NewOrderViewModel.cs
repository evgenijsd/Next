using Next2.Interfaces;
using Next2.Models;
using Next2.Services.Menu;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Tablet
{
    public class NewOrderViewModel : BaseViewModel, IPageActionsHandler
    {
        private readonly IMenuService _menuService;

        private readonly IPopupNavigation _popupNavigation;

        private bool orderByDesc;

        public NewOrderViewModel(
            INavigationService navigationService,
            IMenuService menuService,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _menuService = menuService;
            _popupNavigation = popupNavigation;

            Task.Run(LoadCategoriesAsync);
        }

        #region -- Public properties --

        public ObservableCollection<CategoryModel> CategoriesItems { get; set; }

        public CategoryModel SelectedCategoriesItem { get; set; }

        public ObservableCollection<SetModel> SetsItems { get; set; }

        public ObservableCollection<SubcategoryModel> SubcategoriesItems { get; set; }

        public SubcategoryModel SelectedSubcategoriesItem { get; set; }

        private ICommand _tapSetCommand;
        public ICommand TapSetCommand => _tapSetCommand = new AsyncCommand<SetModel>(OnTapSetCommandAsync);

        private ICommand _tapSortCommand;
        public ICommand TapSortCommand => _tapSortCommand = new AsyncCommand(OnTapSortCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            orderByDesc = false;
            Task.Run(LoadCategoriesAsync);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            SelectedCategoriesItem = new ();
            SelectedSubcategoriesItem = new ();
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

        private async Task OnTapSortCommandAsync()
        {
            orderByDesc = !orderByDesc;
            await LoadSetsAsync();
        }

        private async Task OnTapSetCommandAsync(SetModel set)
        {
            var param = new DialogParameters();
            param.Add(Constants.DialogParameterKeys.SET, set);

            await _popupNavigation.PushAsync(new Views.Tablet.Dialogs.AddSetToOrderDialog(param, async (IDialogParameters obj) => await _popupNavigation.PopAsync()));
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
            if (IsInternetConnected)
            {
                var resultSets = await _menuService.GetSetsAsync(SelectedCategoriesItem.Id, SelectedSubcategoriesItem.Id);

                if (resultSets.IsSuccess)
                {
                    if (orderByDesc)
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
            if (IsInternetConnected)
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

        #endregion
    }
}
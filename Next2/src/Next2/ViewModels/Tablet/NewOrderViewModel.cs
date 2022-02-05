using Next2.Interfaces;
using Next2.Models;
using Next2.Services.Menu;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.ViewModels.Tablet
{
    public class NewOrderViewModel : BaseViewModel, IPageActionsHandler
    {
        private IMenuService _menuService;

        public NewOrderViewModel(
            INavigationService navigationService,
            IMenuService menuService)
            : base(navigationService)
        {
            _menuService = menuService;

            Task.Run(InitStartDataAsync);
        }

        #region -- Public properties --

        public ObservableCollection<CategoryModel> CategoriesItems { get; set; }

        public int SelectedCategoryItemIndex { get; set; }

        public ObservableCollection<SetModel> SetsItems { get; set; }

        public SetModel SelectedSetsItem
        {
            get;
            set;
        }

        public ObservableCollection<SubcategoryModel> SubcategoriesItems { get; set; }

        public int SelectedSubcategoryItemIndex { get; set; }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(SelectedCategoryItemIndex):

                    break;
            }
        }

        #endregion

        #region -- Private methods --

        private async Task InitStartDataAsync()
        {
            if (IsInternetConnected)
            {
                var resultCategories = await _menuService.GetCategoriesAsync();

                if (resultCategories.IsSuccess)
                {
                    CategoriesItems = new (resultCategories.Result);
                    SelectedCategoryItemIndex = CategoriesItems.FirstOrDefault().Id;
                }

                var resultSets = await _menuService.GetSetsAsync(SelectedCategoryItemIndex);

                if (resultSets.IsSuccess)
                {
                    SetsItems = new (resultSets.Result);
                }

                var resultSubcategories = await _menuService.GetSubcategoriesAsync(SelectedCategoryItemIndex);

                if (resultSubcategories.IsSuccess)
                {
                    SubcategoriesItems = new (resultSubcategories.Result);
                    SelectedCategoryItemIndex = SubcategoriesItems.FirstOrDefault().Id;
                }
            }
        }

        #endregion
    }
}
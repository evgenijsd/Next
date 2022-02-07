using Next2.Models;
using Next2.Services.Menu;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class ChooseSetPageViewModel : BaseViewModel
    {
        private bool orderByDesc;

        private IMenuService _menuService;

        private IPopupNavigation _popupNavigation;

        public ChooseSetPageViewModel(
            INavigationService navigationService,
            IMenuService menuService,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _menuService = menuService;
            _popupNavigation = popupNavigation;
        }

        #region -- Public properties --

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

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            if (parameters.ContainsKey(Constants.DialogParameterKeys.CATEGORY))
            {
                CategoryModel category;

                if (parameters.TryGetValue(Constants.DialogParameterKeys.CATEGORY, out category))
                {
                    SelectedCategoriesItem = category;
                }
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

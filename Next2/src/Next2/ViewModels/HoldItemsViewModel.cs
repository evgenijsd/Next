using Next2.Enums;
using Next2.Models;
using Next2.Models.Bindables;
using Next2.Services.HoldItem;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class HoldItemsViewModel : BaseViewModel
    {
        private readonly IHoldItemService _holdItemService;

        private EHoldItemsSortingType _holdItemsSortingType;

        public HoldItemsViewModel(
            IHoldItemService holdItemService,
            INavigationService navigationService)
            : base(navigationService)
        {
            _holdItemService = holdItemService;
        }

        #region -- Public properties --

        public bool IsHoldItemsRefreshing { get; set; }

        public bool IsNothingFound { get; set; }

        public ObservableCollection<HoldItemBindableModel> HoldItems { get; set; } = new();

        private ICommand _refreshHoldItemsCommand;
        public ICommand RefreshHoldItemsCommand => _refreshHoldItemsCommand ??= new AsyncCommand(OnRefreshHoldItemsCommandAsync, allowsMultipleExecutions: false);

        private ICommand _changeSortHoldItemsCommand;
        public ICommand ChangeSortHoldItemsCommand => _changeSortHoldItemsCommand ??= new AsyncCommand<EHoldItemsSortingType>(OnChangeSortHoldItemsCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            IsHoldItemsRefreshing = true;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            HoldItems = new();
        }

        #endregion

        #region -- Private helpers --

        private async Task OnRefreshHoldItemsCommandAsync()
        {
            IsHoldItemsRefreshing = true;
            _holdItemsSortingType = EHoldItemsSortingType.None;
            var holdItems = await _holdItemService.GetHoldItems();

            HoldItems = new(holdItems);
            HoldItems = new(HoldItems.Where(x => x.Id < 9));
            await OnChangeSortHoldItemsCommandAsync(EHoldItemsSortingType.ByTableName);

            IsHoldItemsRefreshing = false;
            IsNothingFound = !HoldItems.Any();
        }

        private Task OnChangeSortHoldItemsCommandAsync(EHoldItemsSortingType holdItemsSortingType)
        {
            if (_holdItemsSortingType == holdItemsSortingType)
            {
                HoldItems = new(HoldItems.Reverse());
            }
            else
            {
                _holdItemsSortingType = holdItemsSortingType;

                var sortedHoldItems = _holdItemService.GetSortedHoldItems(_holdItemsSortingType, HoldItems);

                HoldItems = new(sortedHoldItems);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}

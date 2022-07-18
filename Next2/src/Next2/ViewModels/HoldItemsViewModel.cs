using Next2.Models;
using Next2.Models.Bindables;
using Next2.Services.HoldItem;
using Prism.Navigation;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class HoldItemsViewModel : BaseViewModel
    {
        private readonly IHoldItemService _holdItemService;

        public HoldItemsViewModel(
            IHoldItemService holdItemService,
            INavigationService navigationService)
            : base(navigationService)
        {
            Text = "HoldItems";
            _holdItemService = holdItemService;
        }

        #region -- Public properties --

        public string? Text { get; set; }

        public bool IsHoldItemsRefreshing { get; set; }

        public ObservableCollection<HoldItemBindableModel> HoldItems { get; set; } = new();

        private ICommand _refreshHoldItemsCommand;
        public ICommand RefreshHoldItemsCommand => _refreshHoldItemsCommand ??= new AsyncCommand(OnRefreshHoldItemsCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private async Task OnRefreshHoldItemsCommandAsync()
        {
            var holdItems = await _holdItemService.GetHoldItems();

            HoldItems = holdItems;
        }

        #endregion
    }
}

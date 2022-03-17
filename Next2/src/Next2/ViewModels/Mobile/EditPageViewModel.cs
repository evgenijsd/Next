using Prism.Navigation;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Next2.Models;
using Next2.Services.Order;
using System.Linq;

namespace Next2.ViewModels.Mobile
{
    public class EditPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly int _indexOfSeat;

        public EditPageViewModel(
            INavigationService navigationService,
            IOrderService orderService)
          : base(navigationService)
        {
            _orderService = orderService;

            var seat = _orderService.CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            _indexOfSeat = _orderService.CurrentOrder.Seats.IndexOf(seat);
            SelectedSet = _orderService.CurrentOrder.Seats[_indexOfSeat].SelectedItem;
        }

        #region -- Public properties --

        public SetBindableModel? SelectedSet { get; set; }

        private ICommand _openModifyCommand;
        public ICommand OpenModifyCommand => _openModifyCommand ??= new AsyncCommand(OnOpenModifyCommandAsync);

        private ICommand _openRemoveCommand;
        public ICommand OpenRemoveCommand => _openRemoveCommand ??= new AsyncCommand(OnOpenRemoveCommandAsync);

        private ICommand _openHoldSelectionCommand;
        public ICommand OpenHoldSelectionCommand => _openHoldSelectionCommand ??= new AsyncCommand(OnOpenHoldSelectionCommandAsync);

        #endregion

        #region -- Private helpers --

        private Task OnOpenHoldSelectionCommandAsync()
        {
            return Task.CompletedTask;
        }

        private async Task OnOpenModifyCommandAsync()
        {
            await _navigationService.NavigateAsync(nameof(Views.Mobile.ModificationsPage));
        }

        private Task OnOpenRemoveCommandAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}

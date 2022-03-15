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
        private int _idxSeat;
        private int _idxSet;
        private SetBindableModel _set;
        private SetBindableModel _selectedDish;
        public EditPageViewModel(
            INavigationService navigationService,
            IOrderService orderService)
          : base(navigationService)
        {
            _orderService = orderService;

            var seat = _orderService.CurrentOrder.Seats.FirstOrDefault(row => row.SelectedItem != null);

            _idxSeat = _orderService.CurrentOrder.Seats.IndexOf(seat);
            _set = _orderService.CurrentOrder.Seats[_idxSeat].SelectedItem;
            _idxSet = seat.Sets.IndexOf(_set);
        }

        #region -- Public properties --

        public SetBindableModel SelectedDish { get; set; } = null;

        private ICommand _openModifyCommand;
        public ICommand OpenModifyCommand => _openModifyCommand ??= new AsyncCommand(OnOpenModifyCommandAsync);

        private ICommand _openRemoveCommand;
        public ICommand OpenRemoveCommand => _openRemoveCommand ??= new AsyncCommand(OnOpenRemoveCommandAsync);

        private ICommand _openHoldSelectionCommand;
        public ICommand OpenHoldSelectionCommand => _openHoldSelectionCommand ??= new AsyncCommand(OnOpenHoldSelectionCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            SelectedDish = _orderService.CurrentOrder.Seats[_idxSeat].Sets[_idxSet];
        }

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

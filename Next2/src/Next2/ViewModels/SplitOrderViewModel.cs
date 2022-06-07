using Prism.Navigation;
using System;
using Next2.Models;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Next2.Services.Order;
using System.Linq;
using Rg.Plugins.Popup.Contracts;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;

namespace Next2.ViewModels
{
    public class SplitOrderViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IPopupNavigation _popupNavigation;
        public SplitOrderViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
            _popupNavigation = popupNavigation;
        }

        #region -- Public Properties --

        public FullOrderBindableModel Order { get; set; }

        public DishBindableModel SelectedDish { get; set; }

        public IEnumerable<FullOrderBindableModel> Orders { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        private ICommand _splitByPercentageCommand;
        public ICommand SplitByPercentageCommand => _splitByPercentageCommand ??= new AsyncCommand(OnSplitByPercentageCommand, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Order = _orderService.CurrentOrder;
            var seats = Order.Seats;

            foreach (var seat in seats)
            {
                seat.SetSelectionCommand = new AsyncCommand<object>(OnDishSelectionCommand);
            }

            SelectedDish = Order.Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
            Order.Seats.FirstOrDefault().SelectedItem = Order.Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
        }

        #endregion

        #region -- Private Helpers --

        private async Task OnSplitByPercentageCommand()
        {
            var param = new DialogParameters
            {
            };

            PopupPage popupPage = App.IsTablet
                ? new Views.Tablet.Dialogs.SplitOrderDialog(param, (IDialogParameters dialogResult) => _popupNavigation.PopAsync())
                : new Views.Mobile.Dialogs.SplitOrderDialog(param, (IDialogParameters dialogResult) => _popupNavigation.PopAsync());

            await _popupNavigation.PushAsync(popupPage);
        }

        private Task OnDishSelectionCommand(object? arg)
        {
            var seat = arg as SeatBindableModel;
            SelectedDish = seat.SelectedItem;
            return Task.CompletedTask;
        }

        private async Task OnGoBackCommandAsync()
        {
           await _navigationService.GoBackAsync();
        }

        #endregion

    }
}

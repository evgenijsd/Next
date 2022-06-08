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
using AutoMapper;

namespace Next2.ViewModels
{
    public class SplitOrderViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IPopupNavigation _popupNavigation;
        private readonly IMapper _mapper;
        public SplitOrderViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            IMapper mapper,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
            _popupNavigation = popupNavigation;
            _mapper = mapper;
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
            Order = _mapper.Map<FullOrderBindableModel>(_orderService.CurrentOrder);
            var seats = Order.Seats;

            foreach (var seat in seats)
            {
                seat.SetSelectionCommand = new AsyncCommand<object?>(OnDishSelectionCommand, allowsMultipleExecutions: false);
                seat.Checked = false;
            }

            SelectedDish = Order.Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
            Order.Seats.FirstOrDefault().SelectedItem = Order.Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
            Order.Seats.FirstOrDefault().Checked = true;
        }

        #endregion

        #region -- Private Helpers --

        private async Task OnSplitByPercentageCommand()
        {
            var param = new DialogParameters
            {
                { Constants.DialogParameterKeys.MODEL, Order },
                { Constants.DialogParameterKeys.DISH, SelectedDish },
            };

            PopupPage popupPage = App.IsTablet
                ? new Views.Tablet.Dialogs.SplitOrderDialog(param, (IDialogParameters dialogResult) => _popupNavigation.PopAsync())
                : new Views.Mobile.Dialogs.SplitOrderDialog(param, (IDialogParameters dialogResult) => _popupNavigation.PopAsync());

            await _popupNavigation.PushAsync(popupPage);
        }

        private bool isOneTime = true;
        private Task OnDishSelectionCommand(object? sender)
        {
            if (isOneTime)
            {
                isOneTime = false;
                var seat = sender as SeatBindableModel;

                foreach (var item in Order.Seats.Where(x => x.SeatNumber != seat.SeatNumber))
                {
                    item.SelectedItem = null;
                    item.Checked = false;
                }

                SelectedDish = seat.SelectedItem;
                seat.Checked = true;
                isOneTime = true;
            }

            return Task.CompletedTask;
        }

        private async Task OnGoBackCommandAsync()
        {
           await _navigationService.GoBackAsync();
        }

        #endregion

    }
}

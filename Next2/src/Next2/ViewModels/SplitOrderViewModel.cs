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
using System.Collections.ObjectModel;
using Next2.Enums;
using Next2.Models.Bindables;
using Next2.Models.API.DTO;

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

        public SimpleOrderBindableModel Order { get; set; }

        public DishBindableModel SelectedDish { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        private ICommand _splitByCommand;
        public ICommand SplitByCommand => _splitByCommand ??= new AsyncCommand<ESplitOrderConditions>(OnSplitByCommand, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.ORDER_ID, out Guid id))
            {
                var response = await _orderService.GetOrderByIdAsync(id);

                //foreach (var seat in Order.Seats)
                //{
                //    var newSeat = new SeatBindableModel()
                //    {
                //        SetSelectionCommand = new AsyncCommand<object?>(OnDishSelectionCommand, allowsMultipleExecutions: false),
                //        Checked = false,
                //        SelectedDishes = _mapper.Map<ObservableCollection<DishBindableModel>>(seat.SelectedDishes),
                //    };
                //    Seats.Add(newSeat);
                //}
                SelectedDish = Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
                Seats.FirstOrDefault().SelectedItem = Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
            }
        }

        #endregion

        #region -- Private Helpers --

        private async Task OnSplitByCommand(ESplitOrderConditions condition)
        {
            var param = new DialogParameters
            {
                { Constants.DialogParameterKeys.DESCRIPTION, condition },
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

                foreach (var item in Seats.Where(x => x.SeatNumber != seat.SeatNumber))
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

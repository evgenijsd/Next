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
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class SplitOrderViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly IPopupNavigation _popupNavigation;
        private readonly IMapper _mapper;

        private bool isOneTime = true;

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

        public OrderModelDTO Order { get; set; }

        public DishBindableModel SelectedDish { get; set; }

        public ObservableCollection<SeatBindableModel> Seats { get; set; } = new();

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        private ICommand _splitByCommand;
        public ICommand SplitByCommand => _splitByCommand ??= new AsyncCommand<ESplitOrderConditions>(OnSplitByCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Constants.Navigations.ORDER_ID, out Guid id))
            {
                var response = await _orderService.GetOrderByIdAsync(id);

                if (response.IsSuccess)
                {
                    Order = response.Result;
                    Seats = new(Order.Seats.Select(x => new SeatBindableModel()
                    {
                        IsFirstSeat = x.Id == Order.Seats.First().Id,
                        Checked = false,
                        SeatNumber = x.Number,
                        SetSelectionCommand = new AsyncCommand<object?>(OnDishSelectionCommand),
                        SelectedDishes = new(x.SelectedDishes.Select(y => new DishBindableModel()
                        {
                            DiscountPrice = y.DiscountPrice,
                            DishId = y.DishId,
                            Id = y.Id,
                            ImageSource = y.ImageSource,
                            Name = y.Name,
                            TotalPrice = y.TotalPrice,
                            SelectedDishProportion = y.SelectedDishProportion,
                            SelectedProducts = new(y.SelectedProducts.Select(x => new ProductBindableModel()
                            {
                                Id = x.Id,
                                Price = x.Product.DefaultPrice,
                                Comment = x.Comment,
                                Product = x.Product,
                                AddedIngredients = new(x.AddedIngredients),
                                ExcludedIngredients = new(x.ExcludedIngredients),
                                SelectedIngredients = new(x.SelectedIngredients),
                                SelectedOptions = x.SelectedOptions.FirstOrDefault(),
                            })),
                        })),
                    }).OrderBy(x => x.SeatNumber));
                }

                SelectFirstDish();
            }
        }

        #endregion

        #region -- Private Helpers --

        private async Task OnSplitByCommandAsync(ESplitOrderConditions condition)
        {
            var param = new DialogParameters
            {
                { Constants.DialogParameterKeys.DESCRIPTION, condition },
                { Constants.DialogParameterKeys.SEATS, Seats },
                { Constants.DialogParameterKeys.DISH, SelectedDish },
            };

            PopupPage popupPage = App.IsTablet
                ? new Views.Tablet.Dialogs.SplitOrderDialog(param, SplitOrderDialogCallBack)
                : new Views.Mobile.Dialogs.SplitOrderDialog(param, SplitOrderDialogCallBack);

            await _popupNavigation.PushAsync(popupPage);
        }

        private async void SplitOrderDialogCallBack(IDialogParameters dialogResult)
        {
            if (dialogResult.TryGetValue(Constants.DialogParameterKeys.SEATS, out List<SeatBindableModel> seats))
            {
                foreach (var seat in Seats)
                {
                    var incSeat = seats.FirstOrDefault(x => x.SeatNumber == seat.SeatNumber);

                    if (incSeat is not null)
                    {
                        var dish = SelectedDish.Clone() as DishBindableModel;
                        dish.TotalPrice = incSeat.SelectedItem.TotalPrice;
                        seat.SelectedDishes.Add(dish);
                    }
                }

                if (SelectedDish.TotalPrice == 0)
                {
                    var seat = Seats.FirstOrDefault(x => x.SelectedDishes.Any(x => x.TotalPrice == 0));

                    seat.SelectedDishes.Remove(SelectedDish);

                    if (seat.SelectedDishes.Count == 0)
                    {
                        Seats.Remove(seat);
                    }
                }

                RaisePropertyChanged(nameof(Seats));
                SelectFirstDish();
            }

            if (dialogResult.TryGetValue(Constants.DialogParameterKeys.SPLIT_GROUPS, out List<int[]> groupList))
            {
            }

            await _popupNavigation.PopAsync();
        }

        private void SelectFirstDish()
        {
            SelectedDish = Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
            Seats.FirstOrDefault().SelectedItem = Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
            Seats.FirstOrDefault().Checked = true;
        }

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

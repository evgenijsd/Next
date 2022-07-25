using AutoMapper;
using Next2.Enums;
using Next2.Interfaces;
using Next2.Models.API.DTO;
using Next2.Models.Bindables;
using Next2.Services.Order;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class PrintReceiptViewModel : BaseViewModel, IPageActionsHandler
    {
        private readonly IOrderService _orderService;

        private IEnumerable<SimpleOrderBindableModel> _allClosedOrders;
        private Guid _lastSavedOrderId;

        public PrintReceiptViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
        }

        #region -- Public properties --

        public ObservableCollection<SimpleOrderBindableModel> Orders { get; set; } = new();

        public SimpleOrderBindableModel? SelectedOrder { get; set; }

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    FilterOrdersByDateAsync(value);
                }
            }
        }

        public bool IsNothingFound => IsSearchModeActive && !Orders.Any();

        public bool IsSearchModeActive { get; set; }

        public bool IsOrdersRefreshing { get; set; }

        public bool IsPreloadStateActive => !IsSearchModeActive && (!IsInternetConnected || (IsOrdersRefreshing && !Orders.Any()));

        public EOrdersSortingType OrderSortingType { get; set; }

        private ICommand _refreshOrdersCommand;
        public ICommand RefreshOrdersCommand => _refreshOrdersCommand ??= new AsyncCommand(OnRefreshOrdersCommandAsync);

        private ICommand _selectOrderCommand;
        public ICommand SelectOrderCommand => _selectOrderCommand ??= new Command<SimpleOrderBindableModel?>(OnSelectOrderCommand);

        private ICommand _changeSortOrderCommand;
        public ICommand ChangeSortOrderCommand => _changeSortOrderCommand ??= new Command<EOrdersSortingType>(OnChangeSortOrderCommand);

        #endregion

        #region -- Overrides --

        public override void OnAppearing()
        {
            base.OnAppearing();

            IsSearchModeActive = false;
            IsOrdersRefreshing = true;
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            IsSearchModeActive = false;
            SelectedOrder = null;
            _lastSavedOrderId = Guid.Empty;
        }

        #endregion

        #region -- Private helpers --

        private async Task OnRefreshOrdersCommandAsync()
        {
            bool isOrdersLoaded = false;
            SelectedOrder = null;

            if (IsInternetConnected)
            {
                var gettingOrdersResult = await _orderService.GetOrdersAsync();

                if (gettingOrdersResult.IsSuccess)
                {
                    var closedOrders = gettingOrdersResult.Result
                        .Where(x => x.OrderStatus == EOrderStatus.Closed);

                    var config = new MapperConfiguration(cfg => cfg.CreateMap<SimpleOrderModelDTO, SimpleOrderBindableModel>()
                        .ForMember<string>(x => x.TableNumberOrName, s => s.MapFrom(x => GetTableName(x.TableNumber))));

                    var mapper = new Mapper(config);

                    Orders = mapper.Map<IEnumerable<SimpleOrderModelDTO>, ObservableCollection<SimpleOrderBindableModel>>(closedOrders);

                    _allClosedOrders = Orders.Select(x => x);
                    await MockCloseDateToOrders();
                    await FilterOrdersByDateAsync(SelectedDate);

                    isOrdersLoaded = true;

                    SelectedOrder = _lastSavedOrderId == Guid.Empty
                        ? null
                        : Orders.FirstOrDefault(x => x.Id == _lastSavedOrderId);
                }
                else
                {
                    await ResponseToBadRequestAsync(gettingOrdersResult.Exception.Message);
                }
            }
            else
            {
                await ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
            }

            if (!isOrdersLoaded)
            {
                Orders = new();
            }

            IsOrdersRefreshing = false;
        }

        private Task MockCloseDateToOrders()
        {
            var ordersCount = _allClosedOrders.Count();
            var ordersArray = _allClosedOrders.ToArray();

            var now = DateTime.Now;
            var rand = new Random();

            for (int i = 0; i < ordersCount; i++)
            {
                var randomHours = rand.Next(0, 23);
                var randomMinutes = rand.Next(0, 59);

                if (i < ordersCount / 3)
                {
                    ordersArray[i].Close = new DateTime(now.Year, now.Month, now.Day, randomHours, randomMinutes, 0);
                }
                else if (i < ordersCount / 2)
                {
                    ordersArray[i].Close = new DateTime(now.Year, now.Month, now.Day - 1, randomHours, randomMinutes, 0);
                }
                else
                {
                    ordersArray[i].Close = new DateTime(now.Year, now.Month, now.Day - 2, randomHours, randomMinutes, 0);
                }
            }

            return Task.CompletedTask;
        }

        private Task FilterOrdersByDateAsync(DateTime date)
        {
            Orders = new(_allClosedOrders.Where(x => x.Close.ToShortDateString() == date.ToShortDateString()));
            return Task.CompletedTask;
        }

        private string GetTableName(int? tableNumber)
        {
            return tableNumber == null
                ? LocalizationResourceManager.Current["NotDefined"]
                : $"{LocalizationResourceManager.Current["Table"]} {tableNumber}";
        }

        private void OnSelectOrderCommand(SimpleOrderBindableModel? order)
        {
            SelectedOrder = order == SelectedOrder
                ? null
                : order;
        }

        private void OnChangeSortOrderCommand(EOrdersSortingType orderSortingType)
        {
            if (OrderSortingType == orderSortingType)
            {
                Orders = new(Orders.Reverse());
            }
            else
            {
                OrderSortingType = orderSortingType;

                Orders = new(GetSortedOrders(Orders));
            }
        }

        private IEnumerable<SimpleOrderBindableModel> GetSortedOrders(IEnumerable<SimpleOrderBindableModel> orders)
        {
            EOrdersSortingType orderTabSorting = OrderSortingType == EOrdersSortingType.ByCustomerName
                ? EOrdersSortingType.ByTableNumber
                : OrderSortingType;

            Func<SimpleOrderBindableModel, object> sortingSelector = orderTabSorting switch
            {
                EOrdersSortingType.ByOrderNumber => x => x.Number,
                EOrdersSortingType.ByTableNumber => x => x.TableNumber,
                EOrdersSortingType.ByCustomerName => x => x.TableNumberOrName,
                _ => throw new NotImplementedException(),
            };

            return orders.OrderBy(sortingSelector);
        }

        #endregion

    }
}

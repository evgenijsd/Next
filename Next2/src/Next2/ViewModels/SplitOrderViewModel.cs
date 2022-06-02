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

namespace Next2.ViewModels
{
    public class SplitOrderViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        public SplitOrderViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
        }

        #region -- Public Properties --

        public FullOrderBindableModel Order { get; set; }

        public DishBindableModel SelectedDish { get; set; }

        public IEnumerable<FullOrderBindableModel> Orders { get; set; }

        private ICommand _goBackCommand;

        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        #endregion

        #region -- Overrides --

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Order = _orderService.CurrentOrder;
            SelectedDish = Order.Seats.FirstOrDefault().SelectedDishes.FirstOrDefault();
        }

        #endregion

        #region -- Private Helpers --

        private async Task OnGoBackCommandAsync()
        {
           await _navigationService.GoBackAsync();
        }

        #endregion

    }
}

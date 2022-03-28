using Next2.Enums;
using Next2.Models;
using Next2.Services.CustomersService;
using Next2.Services.Order;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class CustomerRewardsViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly ICustomersService _customersService;

        public CustomerRewardsViewModel(
            INavigationService navigationService,
            IOrderService orderService,
            ICustomersService customersService)
            : base(navigationService)
        {
            _orderService = orderService;
            _customersService = customersService;
        }

        #region -- Public properties --

        public bool IsCustomerSelected { get; set; }

        public ObservableCollection<GiftCardBindabledModel> GiftCards { get; set; }

        public ObservableCollection<GiftCardBindabledModel> SelectedGiftCards { get; set; }

        public ECustomerRewardsPageState PageState { get; set; }

        private ICommand _addNewCustomerCommand;
        public ICommand AddNewCustomerCommand => _addNewCustomerCommand ??= new AsyncCommand(OnAddNewCustomerCommandAsync, allowsMultipleExecutions: false);

        private ICommand _completeCommand;
        public ICommand CompleteCommand => _completeCommand ??= new AsyncCommand(OnCompleteCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (_orderService.CurrentOrder.Customer.Name != string.Empty)
            {
                // check any customer gift cards exist
                bool hasCustomerAnyGiftCards = false;

                IsCustomerSelected = true;

                //PageState = hasCustomerAnyGiftCards
                //    ? ECustomerRewardsPageState.NoRewardsPoints
                //    : ECustomerRewardsPageState.NoRewardSelected;
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnAddNewCustomerCommandAsync()
        {
            IsCustomerSelected = true;

            return Task.CompletedTask;
        }

        private Task OnCompleteCommandAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}

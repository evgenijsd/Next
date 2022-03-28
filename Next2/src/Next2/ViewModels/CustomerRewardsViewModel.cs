using Next2.Enums;
using Next2.Services.Order;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class CustomerRewardsViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        public CustomerRewardsViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
        }

        #region -- Public properties --

        public ECustomerRewardsPageState PageState { get; set; }

        private ICommand _addNewCustomerCommand;
        public ICommand AddNewMemberCommand => _addNewCustomerCommand ??= new AsyncCommand(OnAddNewCustomerCommandAsync, allowsMultipleExecutions: false);

        private ICommand _completeCommand;
        public ICommand CompleteCommand => _completeCommand ??= new AsyncCommand(OnCompleteCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (_orderService.CurrentOrder.CustomerName != string.Empty)
            {
                // check any customer gift cards exist
                bool hasCustomerAnyGiftCards = false;

                PageState = hasCustomerAnyGiftCards
                    ? ECustomerRewardsPageState.NoRewardsPoints
                    : ECustomerRewardsPageState.NoRewardSelected;
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnAddNewCustomerCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnCompleteCommandAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}

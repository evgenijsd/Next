using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Services.CustomersService;
using Next2.Services.Order;
using Next2.Services.Rewards;
using Prism.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        public PaymentViewModel(
            INavigationService navigationService,
            IMapper mapper,
            IOrderService orderService,
            ICustomersService customerService,
            IRewardsService rewardsService)
            : base(navigationService)
        {
            RewardsViewModel = new (navigationService, mapper, orderService, customerService, rewardsService);

            MessagingCenter.Subscribe<SwitchingPaymentStepMessage>(this, Constants.Navigations.SWITCH_PAGE, PageSwitchingMessageHandler);
            MessagingCenter.Subscribe<NavigationMessage>(this, Constants.Navigations.GO_TO_REWARDS_POINTS, GoToRewardsPointsMesasageHandler);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            RewardsViewModel.OnNavigatedTo(parameters);
        }

        #region -- Public properties --

        public EPaymentPageSteps PaymentPageStep { get; set; }

        public RewardsViewModel RewardsViewModel { get; set; }

        #endregion

        #region -- Private helpers --

        private async void GoToRewardsPointsMesasageHandler(NavigationMessage navigationMessage)
        {
            await _navigationService.NavigateAsync(navigationMessage.Path, navigationMessage.Parameters);
        }

        private void PageSwitchingMessageHandler(SwitchingPaymentStepMessage sender)
        {
            if (sender is not null)
            {
                PaymentPageStep = sender.Step;
            }
        }

        #endregion
    }
}
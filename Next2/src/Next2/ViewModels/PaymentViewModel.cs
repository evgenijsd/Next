﻿using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Services.CustomersService;
using Next2.Services.Order;
using Next2.Services.Rewards;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class PaymentViewModel : BaseViewModel
    {
        private readonly IPopupNavigation _popupNavigation;

        public PaymentViewModel(
            INavigationService navigationService,
            IPopupNavigation popupNavigation,
            IMapper mapper,
            IOrderService orderService,
            ICustomersService customerService,
            IRewardsService rewardsService)
            : base(navigationService)
        {
            _popupNavigation = popupNavigation;

            RewardsViewModel = new (
                navigationService,
                popupNavigation,
                mapper,
                orderService,
                customerService,
                rewardsService,
                NavigateAsync,
                GoToPaymentStep);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            RewardsViewModel.OnNavigatedTo(parameters);
        }

        #region -- Public properties --

        public EPaymentPageSteps PaymentPageStep { get; set; }

        public RewardsViewModel RewardsViewModel { get; set; }

        private ICommand _backCancelCommand;
        public ICommand BackCancelCommand => _backCancelCommand ??= new AsyncCommand(OnBackCancelCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private async void NavigateAsync(NavigationMessage navigationMessage) => await _navigationService.NavigateAsync(navigationMessage.Path, navigationMessage.Parameters);

        private void GoToPaymentStep(EPaymentPageSteps step) => PaymentPageStep = step;

        private async Task OnBackCancelCommandAsync()
        {
            if (PaymentPageStep == EPaymentPageSteps.Complete)
            {
                PaymentPageStep = EPaymentPageSteps.Rewards;
            }
            else
            {
                if (RewardsViewModel.IsAnyRewardsApplied)
                {
                    var confirmDialogParameters = new DialogParameters
                    {
                        { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                        { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                        { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["PaymentNotSavedMessage"] },
                        { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Leave"] },
                    };

                    PopupPage confirmDialog = App.IsTablet
                        ? new Views.Tablet.Dialogs.ConfirmDialog(confirmDialogParameters, CloseConfirmExitFromPaymentCallbackAsync)
                        : new Views.Mobile.Dialogs.ConfirmDialog(confirmDialogParameters, CloseConfirmExitFromPaymentCallbackAsync);

                    await _popupNavigation.PushAsync(confirmDialog);
                }
                else
                {
                    await _navigationService.GoBackAsync();
                }
            }
        }

        private async void CloseConfirmExitFromPaymentCallbackAsync(IDialogParameters parameters)
        {
            if (parameters is not null && parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isExitConfirmed))
            {
                await _popupNavigation.PopAsync();

                if (isExitConfirmed)
                {
                    await _navigationService.GoBackAsync();
                }
            }
        }

        #endregion
    }
}
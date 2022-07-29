using AutoMapper;
using Next2.Models;
using Next2.Models.API.Commands;
using Next2.Models.API.DTO;
using Next2.Services.Customers;
using Next2.Services.Notifications;
using Next2.Services.Order;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Mobile
{
    public class InputGiftCardPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly ICustomersService _customersService;
        private readonly IMapper _mapper;
        private readonly INotificationsService _notificationsService;

        public InputGiftCardPageViewModel(
            INavigationService navigationService,
            IOrderService orderService,
            IMapper mapper,
            INotificationsService notificationsService,
            ICustomersService customersService)
            : base(navigationService)
        {
            _orderService = orderService;
            _customersService = customersService;
            _mapper = mapper;
            _notificationsService = notificationsService;

            Customer = _orderService.CurrentOrder.Customer;

            if (Customer is not null && Customer.GiftCards.Any())
            {
                RemainingGiftCardTotal = Customer.GiftCardsTotalFund;
            }
        }

        #region -- Public properties --

        public string InputGiftCardFounds { get; set; }

        public decimal RemainingGiftCardTotal { get; set; }

        public bool IsInSufficientGiftCardFunds { get; set; }

        public bool IsErrorNotificationVisible { get; set; }

        public CustomerBindableModel? Customer { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand = new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        private ICommand _addGiftCardCommand;
        public ICommand AddGiftCardCommand => _addGiftCardCommand = new AsyncCommand(OnAddGiftCardCommandAsync, allowsMultipleExecutions: false);

        private ICommand _displayErrorNotification;
        public ICommand DisplayErrorNotification => _displayErrorNotification = new AsyncCommand(OnDisplayErrorNotificationCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(InputGiftCardFounds))
            {
                IsInSufficientGiftCardFunds = false;
                RemainingGiftCardTotal = 0;

                if (Customer is not null && Customer.GiftCards.Any())
                {
                    RemainingGiftCardTotal = Customer.GiftCardsTotalFund;

                    if (decimal.TryParse(InputGiftCardFounds, out decimal sum))
                    {
                        sum /= 100;

                        if (Customer.GiftCardsTotalFund >= sum)
                        {
                            RemainingGiftCardTotal = Customer.GiftCardsTotalFund - sum;
                        }
                        else
                        {
                            IsInSufficientGiftCardFunds = true;
                            RemainingGiftCardTotal = 0;
                        }
                    }
                }
                else
                {
                    if (decimal.TryParse(InputGiftCardFounds, out decimal sum))
                    {
                        IsInSufficientGiftCardFunds = RemainingGiftCardTotal < sum;
                    }
                }
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnAddGiftCardCommandAsync()
        {
            if (IsInternetConnected)
            {
                if (Customer is not null)
                {
                    IsInSufficientGiftCardFunds = false;

                    RemainingGiftCardTotal = Customer.GiftCardsTotalFund;

                    PopupPage giftCardDialog = new Views.Mobile.Dialogs.AddGiftCardDialog(Customer.GiftCardsId, _customersService, GiftCardViewDialogCallBack);

                    await PopupNavigation.PushAsync(giftCardDialog);
                }
                else
                {
                    await _notificationsService.ShowInfoDialogAsync(
                        LocalizationResourceManager.Current["Error"],
                        LocalizationResourceManager.Current["YouAreNotMember"],
                        LocalizationResourceManager.Current["Ok"]);
                }
            }
            else
            {
                await _notificationsService.ShowInfoDialogAsync(
                    LocalizationResourceManager.Current["Error"],
                    LocalizationResourceManager.Current["NoInternetConnection"],
                    LocalizationResourceManager.Current["Ok"]);
            }
        }

        private async void GiftCardViewDialogCallBack(IDialogParameters parameters)
        {
            await _notificationsService.CloseAllPopupAsync();

            if (parameters.ContainsKey(Constants.DialogParameterKeys.GIFT_CARD_ADDED)
                && parameters.TryGetValue(Constants.DialogParameterKeys.GIFT_GARD, out GiftCardModelDTO giftCard))
            {
                var customerModel = _mapper.Map<CustomerModelDTO>(_orderService.CurrentOrder.Customer);

                var resultOfAddingGiftCard = await _customersService.AddGiftCardToCustomerAsync(customerModel, giftCard);

                if (resultOfAddingGiftCard.IsSuccess)
                {
                    if (Customer is not null)
                    {
                        Customer.GiftCards.Add(giftCard);

                        Customer = new CustomerBindableModel(Customer);

                        RemainingGiftCardTotal = Customer.GiftCardsTotalFund;

                        if (decimal.TryParse(InputGiftCardFounds, out decimal sum))
                        {
                            sum /= 100;

                            RemainingGiftCardTotal = Customer.GiftCardsTotalFund - sum;
                        }
                        else
                        {
                            RemainingGiftCardTotal = Customer.GiftCardsTotalFund;
                        }
                    }
                }
                else
                {
                    await _notificationsService.ResponseToBadRequestAsync(resultOfAddingGiftCard.Exception?.Message);
                }
            }
        }

        private Task OnGoBackCommandAsync()
        {
            var navigationParam = new NavigationParameters();

            if (Customer is not null)
            {
                navigationParam.Add(Constants.Navigations.GIFT_CARD_FOUNDS, InputGiftCardFounds);

                if (Customer.IsUpdatedCustomer)
                {
                    navigationParam.Add(Constants.Navigations.GIFT_CARD_ADDED, true);
                }
            }

            return _navigationService.GoBackAsync(navigationParam);
        }

        private Task OnDisplayErrorNotificationCommandAsync()
        {
            IsErrorNotificationVisible = true;

            Device.BeginInvokeOnMainThread(async () =>
            {
                await Task.Delay(2000);
                IsErrorNotificationVisible = false;
            });

            return Task.CompletedTask;
        }

        #endregion
    }
}

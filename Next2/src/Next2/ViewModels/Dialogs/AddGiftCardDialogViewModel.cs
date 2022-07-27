using Next2.Models;
using Next2.Services.Customers;
using Next2.Services.Order;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Dialogs
{
    public class AddGiftCardDialogViewModel : BindableBase
    {
        private readonly IOrderService _orderService;
        private readonly ICustomersService _customersService;

        public AddGiftCardDialogViewModel(
            IOrderService orderService,
            ICustomersService customersService,
            Action<IDialogParameters> requestClose)
        {
            _orderService = orderService;
            _customersService = customersService;
            Customer = new();
            RequestClose = requestClose;
        }

        #region -- Public properties --

        public string InputGiftCardNumber { get; set; } = string.Empty;

        public CustomerBindableModel? Customer { get; set; }

        public bool IsGiftCardNotExists { get; set; }

        public Action<IDialogParameters> RequestClose;

        private ICommand _closeCommand;
        public ICommand CloseCommand => _closeCommand ??= new AsyncCommand(OnCloseCommandAsync, allowsMultipleExecutions: false);

        private ICommand _addGiftСardCommand;
        public ICommand AddGiftСardCommand => _addGiftСardCommand ??= new AsyncCommand(OnAddGiftCardCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnCloseCommandAsync()
        {
            RequestClose(new DialogParameters());

            return Task.CompletedTask;
        }

        private async Task OnAddGiftCardCommandAsync()
        {
            var dialogParameters = new DialogParameters() { { Constants.DialogParameterKeys.GIFT_CARD_ADDED, true } };

            var giftCardModel = await _customersService.GetGiftCardByNumberAsync(InputGiftCardNumber);

            if (giftCardModel.IsSuccess)
            {
                IsGiftCardNotExists = false;

                var giftCard = giftCardModel.Result;

                if (Customer is not null)
                {
                    var isGiftCardAdded = await _customersService.AddGiftCardToCustomerAsync(Customer, giftCard);

                    if (isGiftCardAdded.IsSuccess)
                    {
                        RequestClose(dialogParameters);
                    }
                    else
                    {
                        IsGiftCardNotExists = true;
                    }
                }
                else
                {
                    var tempCustomerModel = new CustomerBindableModel()
                    {
                        IsUpdatedCustomer = true,
                        IsCustomerRegistrated = false,
                    };

                    tempCustomerModel.GiftCards.Add(giftCard);

                    _orderService.CurrentOrder.Customer = tempCustomerModel;

                    RequestClose(dialogParameters);
                }
            }
            else
            {
                IsGiftCardNotExists = true;
            }
        }

        #endregion
    }
}

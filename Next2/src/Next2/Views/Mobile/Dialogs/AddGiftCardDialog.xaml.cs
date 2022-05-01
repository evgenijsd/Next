using AutoMapper;
using Next2.Services.CustomersService;
using Next2.Services.Order;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class AddGiftCardDialog : PopupPage
    {
        public AddGiftCardDialog(
            DialogParameters param,
            Action<IDialogParameters> requestClose,
            IOrderService orderService,
            ICustomersService customersService)
        {
            InitializeComponent();

            BindingContext = new AddGiftCardDialogViewModel(param, requestClose, orderService, customersService);
        }
    }
}
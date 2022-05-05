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
            IOrderService orderService,
            ICustomersService customersService,
            Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new AddGiftCardDialogViewModel(orderService, customersService, requestClose);
        }
    }
}
using Next2.Services.Customers;
using Next2.Services.Order;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class AddGiftCardDialog : PopupPage
    {
        public AddGiftCardDialog(
            IEnumerable<Guid> giftCardsId,
            ICustomersService customersService,
            Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new AddGiftCardDialogViewModel(giftCardsId, customersService, requestClose);
        }
    }
}
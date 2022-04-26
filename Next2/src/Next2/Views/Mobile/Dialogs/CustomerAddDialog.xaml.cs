﻿using Next2.Services.CustomersService;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class CustomerAddDialog : PopupPage
    {
        public CustomerAddDialog(
            DialogParameters param,
            Action<IDialogParameters> requestClose,
            ICustomersService customersService)
        {
            InitializeComponent();
            BindingContext = new CustomerAddViewModel(param, requestClose, customersService);
        }
    }
}
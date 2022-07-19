﻿using Next2.Services.Customers;
using Next2.ViewModels.Tablet.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class AddNewReservationDialog : PopupPage
    {
        public AddNewReservationDialog(DialogParameters param, Action<IDialogParameters> requestClose, ICustomersService customersService)
        {
            InitializeComponent();
            BindingContext = new AddNewReservationDialogViewModel(param, requestClose, customersService);
        }
    }
}
﻿using Next2.ViewModels.Tablet.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class AddSetToOrderDialog : PopupPage
    {
        public AddSetToOrderDialog(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            BindingContext = new AddSetToOrderDialogViewModel(param, requestClose);
        }
    }
}
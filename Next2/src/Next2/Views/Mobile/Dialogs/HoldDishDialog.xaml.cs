﻿using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class HoldDishDialog : PopupPage
    {
        public HoldDishDialog(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            BindingContext = new HoldDishDialogViewModel(param, requestClose);
        }
    }
}
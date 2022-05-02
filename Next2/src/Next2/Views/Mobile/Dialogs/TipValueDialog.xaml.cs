﻿using AutoMapper;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class TipValueDialog : PopupPage
    {
        public TipValueDialog(Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new TipValueDialogViewModel(requestClose);
        }
    }
}
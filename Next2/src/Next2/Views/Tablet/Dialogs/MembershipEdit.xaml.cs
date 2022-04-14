﻿using AutoMapper;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class MembershipEdit : PopupPage
    {
        public MembershipEdit(DialogParameters param, Action<IDialogParameters> requestClose, IMapper mapper)
        {
            InitializeComponent();

            BindingContext = new MembershipEditViewModel(param, requestClose, mapper);
        }
    }
}
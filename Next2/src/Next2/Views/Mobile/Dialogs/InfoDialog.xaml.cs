using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class InfoDialog : PopupPage
    {
        public InfoDialog(
            DialogParameters parameters,
            Action requestClose)
        {
            InitializeComponent();

            BindingContext = new InfoDialogViewModel(parameters, requestClose);
        }
    }
}
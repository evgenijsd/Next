using Next2.ViewModels.Tablet.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class InputDialog : PopupPage
    {
        public InputDialog(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            BindingContext = new InputDialogViewModel(param, requestClose);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            searchEntry.Focus();
        }
    }
}
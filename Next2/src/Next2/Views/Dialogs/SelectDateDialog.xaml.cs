using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Dialogs
{
    public partial class SelectDateDialog : PopupPage
    {
        public SelectDateDialog(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new SelectDateDialogViewModel(parameters, requestClose);
        }
    }
}
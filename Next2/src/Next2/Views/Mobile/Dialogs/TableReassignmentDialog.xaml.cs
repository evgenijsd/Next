using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class TableReassignmentDialog : PopupPage
    {
        public TableReassignmentDialog(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new TableReassignmentDialogViewModel(parameters, requestClose);
        }
    }
}

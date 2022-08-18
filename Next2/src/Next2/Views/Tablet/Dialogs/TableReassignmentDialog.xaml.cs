using Next2.ViewModels;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class TableReassignmentDialog : PopupPage
    {
        public TableReassignmentDialog(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new TableReassignmentDialogViewModel(param, requestClose);
        }
    }
}
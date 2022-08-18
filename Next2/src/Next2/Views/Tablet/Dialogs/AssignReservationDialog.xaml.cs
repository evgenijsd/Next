using Next2.ViewModels.Tablet.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class AssignReservationDialog : PopupPage
    {
        public AssignReservationDialog(Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new AssignReservationDialogViewModel(requestClose);
        }
    }
}
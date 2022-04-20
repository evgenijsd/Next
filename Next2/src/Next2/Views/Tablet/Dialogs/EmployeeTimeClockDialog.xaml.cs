using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class EmployeeTimeClockDialog : PopupPage
    {
        public EmployeeTimeClockDialog(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            BindingContext = new EmployeeTimeClockViewModel(param, requestClose);
        }
    }
}
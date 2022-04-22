using Next2.Services.Authentication;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class EmployeeTimeClockDialog : PopupPage
    {
        public EmployeeTimeClockDialog(IAuthenticationService authenticationService, DialogParameters param, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            BindingContext = new EmployeeTimeClockViewModel(authenticationService, param, requestClose);
        }
    }
}
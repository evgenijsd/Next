using Next2.Services.Authentication;
using Next2.Services.Log;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class EmployeeTimeClockDialog : PopupPage
    {
        public EmployeeTimeClockDialog(ILogService logService, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            BindingContext = new EmployeeTimeClockViewModel(logService, requestClose);
        }
    }
}
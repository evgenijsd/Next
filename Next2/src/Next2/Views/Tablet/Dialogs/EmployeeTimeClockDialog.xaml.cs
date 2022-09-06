using Next2.Services.WorkLog;
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class EmployeeTimeClockDialog : PopupPage
    {
        public EmployeeTimeClockDialog(
            IWorkLogService workLogService,
            Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new EmployeeTimeClockViewModel(workLogService, requestClose);
        }
    }
}
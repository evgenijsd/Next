using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class ProgrammDeviceDialog : PopupPage
    {
        public ProgrammDeviceDialog(Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new ProgrammDeviceDialogViewModel(requestClose);
        }
    }
}
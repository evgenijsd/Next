using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class DeleteOrderDialog : PopupPage
    {
        public DeleteOrderDialog(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            BindingContext = new DeleteOrderViewModel(param, requestClose);
        }
    }
}
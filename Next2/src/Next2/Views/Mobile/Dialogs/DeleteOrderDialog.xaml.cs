using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class DeleteOrderDialog : PopupPage
    {
        public DeleteOrderDialog(IPopupNavigation popupNavigation, DialogParameters param, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            BindingContext = new DeleteOrderViewModel(popupNavigation, param, requestClose);
        }
    }
}
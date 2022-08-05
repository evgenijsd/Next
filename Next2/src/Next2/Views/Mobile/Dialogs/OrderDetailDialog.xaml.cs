using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class OrderDetailDialog : PopupPage
    {
        public OrderDetailDialog(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new OrderDetailDialogViewModel(param, requestClose);
        }
    }
}
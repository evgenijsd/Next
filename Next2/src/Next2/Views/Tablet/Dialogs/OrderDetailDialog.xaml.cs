using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class OrderDetailDialog : PopupPage
    {
        public OrderDetailDialog(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new OrderDetailDialogViewModel(parameters, requestClose);
        }
    }
}
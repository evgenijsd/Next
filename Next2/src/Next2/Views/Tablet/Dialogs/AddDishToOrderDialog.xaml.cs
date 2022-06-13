using Next2.ViewModels;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class AddDishToOrderDialog : PopupPage
    {
        public AddDishToOrderDialog(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            BindingContext = new AddDishToOrderDialogViewModel(param, requestClose);
        }
    }
}
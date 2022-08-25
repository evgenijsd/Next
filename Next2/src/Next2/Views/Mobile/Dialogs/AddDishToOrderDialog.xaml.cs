using Next2.ViewModels;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class AddDishToOrderDialog : PopupPage
    {
        public AddDishToOrderDialog(
            DialogParameters parameters,
            Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new AddDishToOrderDialogViewModel(parameters, requestClose);
        }
    }
}
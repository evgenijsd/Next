using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
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
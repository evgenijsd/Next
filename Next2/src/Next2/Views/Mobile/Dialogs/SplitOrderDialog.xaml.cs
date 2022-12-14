using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Mobile.Dialogs
{
    public partial class SplitOrderDialog : PopupPage
    {
        public SplitOrderDialog(
            DialogParameters param,
            Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            BindingContext = new SplitOrderDialogViewModel(param, requestClose);
        }
    }
}
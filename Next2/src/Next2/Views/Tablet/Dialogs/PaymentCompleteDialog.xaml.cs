using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class PaymentCompleteDialog : PopupPage
    {
        public PaymentCompleteDialog(Action<IDialogParameters> requestClose)
        {
            InitializeComponent();

            BindingContext = new PaymentCompleteDialogViewModel(requestClose);
        }
    }
}
using Next2.ViewModels.Dialogs;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class CustomerInfoDialogTab : PopupPage
    {
        public CustomerInfoDialogTab(DialogParameters param, Action<IDialogParameters> requestClose)
        {
            InitializeComponent();
            mainFrame.WidthRequest = Prism.PrismApplicationBase.Current.MainPage.Width / 3;
            mainFrame.HeightRequest = Prism.PrismApplicationBase.Current.MainPage.Height * 0.84;
            BindingContext = new CustomerInfoViewModel(param, requestClose);
        }
    }
}
using Next2.Services.Membership;
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
    public partial class MembershipEditDialog : PopupPage
    {
        public MembershipEditDialog(DialogParameters param, Action<IDialogParameters> requestClose, IMembershipService membershipService)
        {
            InitializeComponent();
            BindingContext = new MembershipEditDialogViewModel(param, requestClose, membershipService);
        }
    }
}
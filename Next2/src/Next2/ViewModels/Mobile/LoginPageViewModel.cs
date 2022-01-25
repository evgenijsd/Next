using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.ViewModels.Mobile
{
    public class LoginPageViewModel : BaseViewModel
    {
        public LoginPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        public DateTime CurrentDate = DateTime.Now;
    }
}

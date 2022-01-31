using Prism.Navigation;

namespace Next2.ViewModels
{
    public class MembershipViewModel : BaseViewModel
    {
        public MembershipViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "Membership";
        }

        #region -- Public properties --

        public string? Text { get; set; }

        #endregion
    }
}

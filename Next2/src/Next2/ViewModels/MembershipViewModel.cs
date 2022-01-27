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

        public string? Text { get; set; }
    }
}

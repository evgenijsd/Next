using Next2.Enums;
using Next2.Models;
using Next2.Services.Membership;
using Prism.Navigation;
using System.Collections.ObjectModel;

namespace Next2.ViewModels.Tablet
{
    public class MembershipViewModel : BaseViewModel
    {
        private IMembershipService _membershipService;

        public MembershipViewModel(
            INavigationService navigationService,
            IMembershipService membershipService)
            : base(navigationService)
        {
            _membershipService = membershipService;
        }

        #region -- Public properties --

        public string Text { get; set; }

        public ObservableCollection<MemberBindableModel> Members { get; set; }

        public ESortingType SortingTypeMembers { get; set; }

        public EMemberSorting CurrentMemberSorting { get; set; }

        public MemberModel SelectedMember { get; set; }

        #endregion
    }
}

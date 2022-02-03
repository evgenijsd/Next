using Next2.Enums;
using Next2.Extensions;
using Next2.Models;
using Next2.Services.Membership;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

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

            _ = RefreshMembersAsync();
        }

        #region -- Public properties --

        public ObservableCollection<MemberBindableModel> Members { get; set; }

        public bool IsMembersRefreshing { get; set; }

        public ESortingOrder MembersSortingOrder { get; set; } = ESortingOrder.ByAscending;

        public EMemberSorting MemberSorting { get; set; } = EMemberSorting.ByMembershipStartTime;

        private ICommand _refreshMembersCommand;
        public ICommand RefreshMembersCommand => _refreshMembersCommand ??= new AsyncCommand(OnRefreshMembersCommandAsync);

        private ICommand _memberSortingChangeCommand;
        public ICommand MemberSortingChangeCommand => _memberSortingChangeCommand ??= new AsyncCommand<EMemberSorting>(OnMemberSortingChangeCommandAsync);

        #endregion

        #region -- Private helpers --

        private void ReverseMembersSortingOrder()
        {
            MembersSortingOrder = MembersSortingOrder == ESortingOrder.ByAscending
                ? ESortingOrder.ByDescending
                : ESortingOrder.ByAscending;
        }

        private IEnumerable<MemberBindableModel> GetSortedMembers(IEnumerable<MemberBindableModel> members)
        {
            IEnumerable<MemberBindableModel> sortedMembers = null;

            Func<MemberBindableModel, object> comparer = null;

            switch (MemberSorting)
            {
                case EMemberSorting.ByCustomerName:
                    comparer = x => x.CustomerName;
                    break;
                case EMemberSorting.ByMembershipStartTime:
                    comparer = x => x.MembershipStartTime;
                    break;
                case EMemberSorting.ByMembershipEndTime:
                    comparer = x => x.MembershipEndTime;
                    break;
            }

            sortedMembers = MembersSortingOrder == ESortingOrder.ByAscending
                ? members.OrderBy(comparer)
                : members.OrderByDescending(comparer);

            return sortedMembers;
        }

        private async Task RefreshMembersAsync()
        {
            IsMembersRefreshing = true;

            var membershipResult = await _membershipService.GetAllMembersAsync();

            if (membershipResult.IsSuccess)
            {
                var allMembers = membershipResult.Result;

                var memberBindableModels = new ObservableCollection<MemberBindableModel>(allMembers.Select(x => x.ToBindableModel()));

                var sortedmemberBindableModels = GetSortedMembers(memberBindableModels);

                Members = new ObservableCollection<MemberBindableModel>(sortedmemberBindableModels);

                IsMembersRefreshing = false;
            }
        }

        private async Task OnRefreshMembersCommandAsync()
        {
            await RefreshMembersAsync();
        }

        private Task OnMemberSortingChangeCommandAsync(EMemberSorting memberSorting)
        {
            if (MemberSorting == memberSorting)
            {
                ReverseMembersSortingOrder();
            }
            else
            {
                MemberSorting = memberSorting;
            }

            var sortedMembers = GetSortedMembers(Members);

            Members = new ObservableCollection<MemberBindableModel>(sortedMembers);

            return Task.CompletedTask;
        }

        #endregion
    }
}
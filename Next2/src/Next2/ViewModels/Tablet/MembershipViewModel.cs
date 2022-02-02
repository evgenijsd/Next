using Next2.Enums;
using Next2.Extensions;
using Next2.Models;
using Next2.Services.Membership;
using Prism.Navigation;
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

        public ObservableCollection<MemberBindableModel>? MembersToDisplay { get; set; }

        public MemberModel? SelectedMember { get; set; }

        public bool IsMembersRefreshing { get; set; }

        public ESortingOrder TypeOfMemberSortingOrder { get; set; } = ESortingOrder.ByAscending;

        public EMemberSorting CurrentMemberSortingCriterion { get; set; } = EMemberSorting.ByMembershipStartTime;

        private ICommand _refreshMembersCommand;
        public ICommand RefreshMembersCommand => _refreshMembersCommand ??= new AsyncCommand(OnRefreshMembersCommandAsync);

        public ICommand _tableHeaderColumnTapCommand;
        public ICommand TableHeaderColumnTapCommand => _tableHeaderColumnTapCommand ??= new AsyncCommand<EMemberSorting>(OnTableHeaderColumnTapCommandAsync);

        #endregion

        #region -- Private helpers --

        private void ReverseSortingTypeOfMembers()
        {
            TypeOfMemberSortingOrder = TypeOfMemberSortingOrder == ESortingOrder.ByDescending
                ? ESortingOrder.ByAscending
                : ESortingOrder.ByDescending;
        }

        private IEnumerable<MemberBindableModel> GetSortedMembers(IEnumerable<MemberBindableModel> members)
        {
            IEnumerable<MemberBindableModel> sortedMembers = null;

            switch (TypeOfMemberSortingOrder)
            {
                case ESortingOrder.ByAscending:

                    switch (CurrentMemberSortingCriterion)
                    {
                        case EMemberSorting.ByCustomerName:
                            sortedMembers = members.OrderBy(x => x.CustomerName);
                            break;
                        case EMemberSorting.ByMembershipStartTime:
                            sortedMembers = members.OrderBy(x => x.MembershipStartTime);
                            break;
                        case EMemberSorting.ByMembershipEndTime:
                            sortedMembers = members.OrderBy(x => x.MembershipEndTime);
                            break;
                    }

                    break;

                case ESortingOrder.ByDescending:

                    switch (CurrentMemberSortingCriterion)
                    {
                        case EMemberSorting.ByCustomerName:
                            sortedMembers = members.OrderByDescending(x => x.CustomerName);
                            break;
                        case EMemberSorting.ByMembershipStartTime:
                            sortedMembers = members.OrderByDescending(x => x.MembershipStartTime);
                            break;
                        case EMemberSorting.ByMembershipEndTime:
                            sortedMembers = members.OrderByDescending(x => x.MembershipEndTime);
                            break;
                    }

                    break;
            }

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

                MembersToDisplay = new ObservableCollection<MemberBindableModel>(sortedmemberBindableModels);

                IsMembersRefreshing = false;
            }
        }

        private async Task OnRefreshMembersCommandAsync()
        {
            await RefreshMembersAsync();
        }

        private Task OnTableHeaderColumnTapCommandAsync(EMemberSorting sortingCriterion)
        {
            bool isCurrentMemberSortingCriterionChanged = false;

            switch (sortingCriterion)
            {
                case EMemberSorting.ByCustomerName:
                    isCurrentMemberSortingCriterionChanged = CurrentMemberSortingCriterion != EMemberSorting.ByCustomerName;
                    break;
                case EMemberSorting.ByMembershipStartTime:
                    isCurrentMemberSortingCriterionChanged = CurrentMemberSortingCriterion != EMemberSorting.ByMembershipStartTime;
                    break;
                case EMemberSorting.ByMembershipEndTime:
                    isCurrentMemberSortingCriterionChanged = CurrentMemberSortingCriterion != EMemberSorting.ByMembershipEndTime;
                    break;
            }

            if (!isCurrentMemberSortingCriterionChanged)
            {
                ReverseSortingTypeOfMembers();
            }

            CurrentMemberSortingCriterion = sortingCriterion;

            var sortedMembers = GetSortedMembers(MembersToDisplay);

            MembersToDisplay = new ObservableCollection<MemberBindableModel>(sortedMembers);

            return Task.CompletedTask;
        }

        #endregion
    }
}

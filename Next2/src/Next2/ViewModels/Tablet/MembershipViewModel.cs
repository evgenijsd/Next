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

        public ETypeSortingOrder TypeOfMemberSortingOrder { get; set; } = ETypeSortingOrder.ByAscending;

        public EMemberSortingCriterion CurrentMemberSortingCriterion { get; set; } = EMemberSortingCriterion.ByMembershipStartTime;

        private ICommand _refreshMembersCommand;
        public ICommand RefreshMembersCommand => _refreshMembersCommand ??= new AsyncCommand(OnRefreshMembersCommandAsync);

        public ICommand _tableHeaderColumnTapCommand;
        public ICommand TableHeaderColumnTapCommand => _tableHeaderColumnTapCommand ??= new AsyncCommand<EMemberSortingCriterion>(OnTableHeaderColumnTapCommandAsync);

        #endregion

        #region -- Private helpers --

        private void ReverseSortingTypeOfMembers()
        {
            TypeOfMemberSortingOrder = TypeOfMemberSortingOrder == ETypeSortingOrder.ByDescending
                ? ETypeSortingOrder.ByAscending
                : ETypeSortingOrder.ByDescending;
        }

        private IEnumerable<MemberBindableModel> GetSortedMembers(IEnumerable<MemberBindableModel> members)
        {
            IEnumerable<MemberBindableModel> sortedMembers = null;

            switch (TypeOfMemberSortingOrder)
            {
                case ETypeSortingOrder.ByAscending:

                    switch (CurrentMemberSortingCriterion)
                    {
                        case EMemberSortingCriterion.ByCustomerName:
                            sortedMembers = members.OrderBy(x => x.CustomerName);
                            break;
                        case EMemberSortingCriterion.ByMembershipStartTime:
                            sortedMembers = members.OrderBy(x => x.MembershipStartTime);
                            break;
                        case EMemberSortingCriterion.ByMembershipEndTime:
                            sortedMembers = members.OrderBy(x => x.MembershipEndTime);
                            break;
                    }

                    break;

                case ETypeSortingOrder.ByDescending:

                    switch (CurrentMemberSortingCriterion)
                    {
                        case EMemberSortingCriterion.ByCustomerName:
                            sortedMembers = members.OrderByDescending(x => x.CustomerName);
                            break;
                        case EMemberSortingCriterion.ByMembershipStartTime:
                            sortedMembers = members.OrderByDescending(x => x.MembershipStartTime);
                            break;
                        case EMemberSortingCriterion.ByMembershipEndTime:
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

            var getAllMembersAsyncResult = await _membershipService.GetAllMembersAsync();

            if (getAllMembersAsyncResult.IsSuccess)
            {
                var allMembers = getAllMembersAsyncResult.Result;

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

        private Task OnTableHeaderColumnTapCommandAsync(EMemberSortingCriterion sortingCriterion)
        {
            bool isCurrentMemberSortingCriterionChanged = false;

            switch (sortingCriterion)
            {
                case EMemberSortingCriterion.ByCustomerName:
                    isCurrentMemberSortingCriterionChanged = CurrentMemberSortingCriterion != EMemberSortingCriterion.ByCustomerName;
                    break;
                case EMemberSortingCriterion.ByMembershipStartTime:
                    isCurrentMemberSortingCriterionChanged = CurrentMemberSortingCriterion != EMemberSortingCriterion.ByMembershipStartTime;
                    break;
                case EMemberSortingCriterion.ByMembershipEndTime:
                    isCurrentMemberSortingCriterionChanged = CurrentMemberSortingCriterion != EMemberSortingCriterion.ByMembershipEndTime;
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

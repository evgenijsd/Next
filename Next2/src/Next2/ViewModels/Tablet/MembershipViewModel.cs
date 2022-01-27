using Next2.Enums;
using Next2.Extensions;
using Next2.Helpers;
using Next2.Models;
using Next2.Services;
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
        private IMockService _mockService;

        public MembershipViewModel(
            INavigationService navigationService,
            IMembershipService membershipService,
            IMockService mockService)
            //IStatusBarHelper statusBarHelper)
            : base(navigationService)
        {
            _mockService = mockService;
            _membershipService = membershipService;
            //statusBarHelper.HideStatusBar();
            CurrentMemberSortingCriterion = EMemberSortingCriterion.ByCustomerName;
            TypeOfMemberSortingOrder = ETypeSortingOrder.ByAscending;
        }

        #region -- Public properties --

        public ObservableCollection<MemberBindableModel>? Members { get; set; }

        public MemberModel? SelectedMember { get; set; }

        public bool IsMembersRefreshing { get; set; }

        public ETypeSortingOrder TypeOfMemberSortingOrder { get; set; }

        public EMemberSortingCriterion CurrentMemberSortingCriterion { get; set; }

        private ICommand _memberTapCommand;
        public ICommand MemberTapCommand => _memberTapCommand ??= new AsyncCommand<MemberBindableModel>(OnMemberTapCommandAsync);

        private ICommand _pullToRefreshMembersCommand;
        public ICommand PullToRefreshMembersCommand => _pullToRefreshMembersCommand ??= new AsyncCommand(OnPullToRefreshMembersCommandAsync);

        public ICommand _tableHeaderTapCommand;
        public ICommand TableHeaderTapCommand => _tableHeaderTapCommand ??= new AsyncCommand<EMemberSortingCriterion>(OnTableheaderTapCommandAsync);

        public ICommand _testCommand;
        public ICommand TestCommand => _testCommand ??= new AsyncCommand(async () =>
        {
            _mockService.AddAsync(new MemberModel
            {
                CustomerName = "Andrii Tantsiura",
                Phone = "123-456-7890",
                MembershipStartTime = DateTime.Now,
                MembershipEndTime = DateTime.Now,
            });
        });

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await RefreshMembersAsync();
        }

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

                Members = new ObservableCollection<MemberBindableModel>(sortedmemberBindableModels);

                IsMembersRefreshing = false;
            }
        }

        private Task OnMemberTapCommandAsync(MemberBindableModel selectedMember)
        {
            return Task.CompletedTask;
        }

        private async Task OnPullToRefreshMembersCommandAsync()
        {
            await RefreshMembersAsync();
        }

        private Task OnTableheaderTapCommandAsync(EMemberSortingCriterion sortingCriterion)
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

            var sortedMembers = GetSortedMembers(Members);

            Members = new ObservableCollection<MemberBindableModel>(sortedMembers);

            return Task.CompletedTask;
        }

        #endregion
    }
}

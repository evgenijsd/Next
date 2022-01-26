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
            IMockService mockService,
            IStatusBarHelper statusBarHelper)
            : base(navigationService)
        {
            _mockService = mockService;
            _membershipService = membershipService;
            statusBarHelper.HideStatusBar();

            SortingTypeMembers = ESortingType.ByAscending;
            CurrentMemberSorting = EMemberSorting.ByCustomerName;
        }

        #region -- Public properties --

        public ObservableCollection<MemberBindableModel>? Members { get; set; }

        public MemberModel? SelectedMember { get; set; }

        public bool IsMembersRefreshing { get; set; }

        public ESortingType SortingTypeMembers { get; set; }

        public EMemberSorting CurrentMemberSorting { get; set; }

        private ICommand _memberTapCommand;
        public ICommand MemberTapCommand => _memberTapCommand ??= new AsyncCommand(OnMemberTapCommandAsync);

        private ICommand _pullToRefreshMembersCommand;
        public ICommand PullToRefreshMembersCommand => _pullToRefreshMembersCommand ??= new AsyncCommand(OnPullToRefreshMembersCommandAsync);

        public ICommand _sortingByCustomerNameTapCommand;
        public ICommand SortingByCustomerNameTapCommand => _sortingByCustomerNameTapCommand ??= new AsyncCommand(OnSortingByCustomerNameTapCommandAsync);

        public ICommand _sortingByMembershipStartTimeTapCommand;
        public ICommand SortingByMembershipStartTimeTapCommand => _sortingByMembershipStartTimeTapCommand ??= new AsyncCommand(OnSortingByMembershipStartTimeTapCommandAsync);

        public ICommand _sortingByMembershipEndTimeTapCommand;
        public ICommand SortingByMembershipEndTimeTapCommand => _sortingByMembershipEndTimeTapCommand ??= new AsyncCommand(OnSortingByMembershipEndTimeTapCommandAsync);

        public ICommand _testCommand;
        public ICommand TestCommand => _testCommand ??= new AsyncCommand(OnTestCommandAsync);

        private Task OnTestCommandAsync()
        {
            _mockService.AddAsync(new MemberModel
            {
                CustomerName = "Andrii Tantsiura",
                Phone = "123-456-7890",
                MembershipStartTime = DateTime.Now,
                MembershipEndTime = DateTime.Now,
            });

            return Task.CompletedTask;
        }

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            await RefreshMembersAsync();
        }

        #endregion

        #region -- Private helpers --

        private IEnumerable<MemberBindableModel> GetSortedMembers(IEnumerable<MemberBindableModel> members)
        {
            IEnumerable<MemberBindableModel> sortedMembers = null;

            switch (SortingTypeMembers)
            {
                case ESortingType.ByAscending:

                    switch (CurrentMemberSorting)
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

                case ESortingType.ByDescending:

                    switch (CurrentMemberSorting)
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

        private void ReverseSortingTypeOfMembers()
        {
            SortingTypeMembers = SortingTypeMembers == ESortingType.ByDescending
                ? ESortingType.ByAscending
                : ESortingType.ByDescending;
        }

        private async Task RefreshMembersAsync()
        {
            IsMembersRefreshing = true;

            var getAllMembersAsyncResult = await _membershipService.GetAllMembersAsync();

            if (getAllMembersAsyncResult.IsSuccess)
            {
                var allMembers = getAllMembersAsyncResult.Result;

                Members = new ObservableCollection<MemberBindableModel>(allMembers.Select(x => x.ToBindableModel()));

                RefreshMembersView();

                IsMembersRefreshing = false;
            }
        }

        private void RefreshMembersView()
        {
            var sortedMembers = GetSortedMembers(Members);

            Members = new ObservableCollection<MemberBindableModel>(sortedMembers);
        }

        private Task OnMemberTapCommandAsync()
        {
            return Task.CompletedTask;
        }

        private async Task OnPullToRefreshMembersCommandAsync()
        {
            await RefreshMembersAsync();
        }

        private Task OnSortingByCustomerNameTapCommandAsync()
        {
            if (CurrentMemberSorting == EMemberSorting.ByCustomerName)
            {
                ReverseSortingTypeOfMembers();
            }
            else
            {
                CurrentMemberSorting = EMemberSorting.ByCustomerName;
            }

            RefreshMembersView();

            return Task.CompletedTask;
        }

        private Task OnSortingByMembershipStartTimeTapCommandAsync()
        {
            if (CurrentMemberSorting == EMemberSorting.ByMembershipStartTime)
            {
                ReverseSortingTypeOfMembers();
            }
            else
            {
                CurrentMemberSorting = EMemberSorting.ByMembershipStartTime;
            }

            RefreshMembersView();

            return Task.CompletedTask;
        }

        private Task OnSortingByMembershipEndTimeTapCommandAsync()
        {
            if (CurrentMemberSorting == EMemberSorting.ByMembershipEndTime)
            {
                ReverseSortingTypeOfMembers();
            }
            else
            {
                CurrentMemberSorting = EMemberSorting.ByMembershipEndTime;
            }

            RefreshMembersView();

            return Task.CompletedTask;
        }

        #endregion
    }
}

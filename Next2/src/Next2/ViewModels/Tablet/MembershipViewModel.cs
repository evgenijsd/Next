﻿using Next2.Enums;
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
        }

        #region -- Public properties --

        public ObservableCollection<MemberBindableModel> Members { get; set; }

        public bool IsMembersRefreshing { get; set; }

        public EMemberSorting MemberSorting { get; set; }

        private ICommand _refreshMembersCommand;
        public ICommand RefreshMembersCommand => _refreshMembersCommand ??= new AsyncCommand(OnRefreshMembersCommandAsync);

        private ICommand _memberSortingChangeCommand;
        public ICommand MemberSortingChangeCommand => _memberSortingChangeCommand ??= new AsyncCommand<EMemberSorting>(OnMemberSortingChangeCommandAsync);

        #endregion

        #region -- Overrides --

        public override async void OnAppearing()
        {
            MemberSorting = EMemberSorting.ByMembershipStartTime;

            await RefreshMembersAsync();

            base.OnAppearing();
        }

        public override void OnDisappearing()
        {
            Members = null;

            base.OnDisappearing();
        }

        #endregion

        #region -- Private helpers --

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

            sortedMembers = members.OrderBy(comparer);

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
                var reversedMembers = Members.Reverse();

                Members = new ObservableCollection<MemberBindableModel>(reversedMembers);
            }
            else
            {
                MemberSorting = memberSorting;

                var sortedMembers = GetSortedMembers(Members);

                Members = new ObservableCollection<MemberBindableModel>(sortedMembers);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
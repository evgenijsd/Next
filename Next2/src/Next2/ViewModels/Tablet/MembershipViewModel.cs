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
            Members.Clear();

            base.OnDisappearing();
        }

        #endregion

        #region -- Private helpers --

        private IEnumerable<MemberBindableModel> GetSortedMembers(IEnumerable<MemberBindableModel> members)
        {
            Func<MemberBindableModel, object> comparer = MemberSorting switch
            {
                EMemberSorting.ByMembershipStartTime => x => x.MembershipStartTime,
                EMemberSorting.ByMembershipEndTime => x => x.MembershipEndTime,
                _ => x => x.CustomerName,
            };

            return members.OrderBy(comparer);
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

                Members = new (sortedmemberBindableModels);

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
                Members = new (Members.Reverse());
            }
            else
            {
                MemberSorting = memberSorting;

                var sortedMembers = GetSortedMembers(Members);

                Members = new (sortedMembers);
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
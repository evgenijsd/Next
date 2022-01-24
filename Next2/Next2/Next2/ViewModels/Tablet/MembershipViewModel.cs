using Next2.Enums;
using Next2.Extensions;
using Next2.Models;
using Next2.Services.Membership;
using Next2.Views;
using Prism.Navigation;
using System;
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

            IsMembersRefreshing = true;
            SortingTypeMembers = ESortingType.ByAscending;
            CurrentMemberSorting = EMemberSorting.ByName;
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

        public ICommand _changeMemberSortingTapCommand;
        public ICommand ChangeMemberSortingTapCommand => _changeMemberSortingTapCommand ??= new AsyncCommand(OnChangeMemberSortingTapCommandAsync);

        #endregion

        #region -- Overrides --

        public override async Task InitializeAsync(INavigationParameters parameters)
        {
            if (IsConnectionExist)
            {
                await InitMembersAsync();
            }
            else
            {
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (IsConnectionExist)
            {
                _ = InitMembersAsync(); // mb some OnNavigationToAsync?
            }
            else
            {
            }

            base.OnNavigatedTo(parameters);
        }

        #endregion

        #region -- Private helpers --

        private async Task InitMembersAsync()
        {
            IsMembersRefreshing = true;

            var allMembers = await _membershipService.GetAllMembersAsync();

            if (allMembers.IsSuccess)
            {
                Members = new ObservableCollection<MemberBindableModel>(allMembers.Result.Select(x => x.ToBindableModel()));

                IsMembersRefreshing = false;
            }
        }

        private Task OnMemberTapCommandAsync()
        {
            return Task.CompletedTask;
        }

        private Task OnPullToRefreshMembersCommandAsync()
        {
            return InitMembersAsync();
        }

        private Task OnChangeMemberSortingTapCommandAsync()
        {
            return Task.CompletedTask;
        }

        #endregion
    }
}

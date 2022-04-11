using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Services.Membership;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
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
        private readonly IMapper _mapper;
        private readonly IMembershipService _membershipService;
        private readonly IPopupNavigation _popupNavigation;

        public MembershipViewModel(
            IMapper mapper,
            INavigationService navigationService,
            IMembershipService membershipService,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _mapper = mapper;
            _membershipService = membershipService;
            _popupNavigation = popupNavigation;
        }

        #region -- Public properties --

        public ObservableCollection<MemberBindableModel> Members { get; set; } = new ();

        public bool IsMembersRefreshing { get; set; }

        public EMemberSorting MemberSorting { get; set; }

        private ICommand _refreshMembersCommand;
        public ICommand RefreshMembersCommand => _refreshMembersCommand ??= new AsyncCommand(OnRefreshMembersCommandAsync);

        private ICommand _memberSortingChangeCommand;
        public ICommand MemberSortingChangeCommand => _memberSortingChangeCommand ??= new AsyncCommand<EMemberSorting>(OnMemberSortingChangeCommandAsync);

        private ICommand _MembershipEditCommand;
        public ICommand MembershipEditCommand => _MembershipEditCommand ??= new AsyncCommand<MemberBindableModel>(OnMembershipEditCommandAsync, allowsMultipleExecutions: false);

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

            var membersResult = await _membershipService.GetAllMembersAsync();

            if (membersResult.IsSuccess)
            {
                var memberBindableModels = _mapper.Map<ObservableCollection<MemberBindableModel>>(membersResult.Result);

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

        private Task OnMembershipEditCommandAsync(MemberBindableModel member)
        {
            var param = new DialogParameters();

            PopupPage popupPage = new Views.Tablet.Dialogs.MembershipEditDialog(param, MembershipEditDialogCallBack, _membershipService);

            return _popupNavigation.PushAsync(popupPage);
        }

        private async void MembershipEditDialogCallBack(IDialogParameters param)
        {
            await _popupNavigation.PopAsync();

            if (param.TryGetValue("Id", out int customerId))
            {
                await RefreshMembersAsync();

                /*int index = Customers.IndexOf(Customers.FirstOrDefault(x => x.Id == customerId));
                Customers.Move(index, 0);*/
            }
        }

        #endregion
    }
}
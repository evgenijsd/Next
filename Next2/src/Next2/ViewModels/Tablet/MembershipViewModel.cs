using AutoMapper;
using Next2.Enums;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Membership;
using Next2.Views.Tablet.Dialogs;
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
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Tablet
{
    public class MembershipViewModel : BaseViewModel
    {
        private readonly IMapper _mapper;
        private readonly IMembershipService _membershipService;
        private readonly IPopupNavigation _popupNavigation;

        private MemberModel _member;

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
        public ICommand RefreshMembersCommand => _refreshMembersCommand ??= new AsyncCommand(OnRefreshMembersCommandAsync, allowsMultipleExecutions: false);

        private ICommand _memberSortingChangeCommand;
        public ICommand MemberSortingChangeCommand => _memberSortingChangeCommand ??= new AsyncCommand<EMemberSorting>(OnMemberSortingChangeCommandAsync, allowsMultipleExecutions: false);

        private ICommand _MembershipEditCommand;
        public ICommand MembershipEditCommand => _MembershipEditCommand ??= new AsyncCommand<MemberBindableModel?>(OnMembershipEditCommandAsync, allowsMultipleExecutions: false);

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

                foreach (var member in Members)
                {
                    member.TapCommand = MembershipEditCommand;
                }

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

        private async Task OnMembershipEditCommandAsync(MemberBindableModel? member)
        {
            if (member is MemberBindableModel selectedMember)
            {
                var parameters = new DialogParameters { { Constants.DialogParameterKeys.MODEL, selectedMember } };

                PopupPage popupPage = new Views.Tablet.Dialogs.MembershipEditDialog(parameters, MembershipEditDialogCallBack, _mapper);

                await _popupNavigation.PushAsync(popupPage);
            }
        }

        private async void MembershipEditDialogCallBack(IDialogParameters parameters)
        {
            await _popupNavigation.PopAsync();

            if (parameters.TryGetValue(Constants.DialogParameterKeys.UPDATE, out MemberBindableModel member))
            {
                _member = _mapper.Map<MemberBindableModel, MemberModel>(member);

                var confirmDialogParameters = new DialogParameters
                    {
                        { Constants.DialogParameterKeys.CONFIRM_MODE, EConfirmMode.Attention },
                        { Constants.DialogParameterKeys.TITLE, LocalizationResourceManager.Current["AreYouSure"] },
                        { Constants.DialogParameterKeys.DESCRIPTION, LocalizationResourceManager.Current["MembershipUpdate"] },
                        { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, LocalizationResourceManager.Current["Cancel"] },
                        { Constants.DialogParameterKeys.OK_BUTTON_TEXT, LocalizationResourceManager.Current["Ok"] },
                    };
                PopupPage confirmDialog = new ConfirmDialog(confirmDialogParameters, CloseConfirmDialogUpdateCallback);
                await _popupNavigation.PushAsync(confirmDialog);
            }
        }

        private async void CloseConfirmDialogUpdateCallback(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isMembershipDisableAccepted))
            {
                if (isMembershipDisableAccepted)
                {
                    await _membershipService.SaveMemberAsync(_member);

                    await RefreshMembersAsync();
                }
            }

            await _popupNavigation.PopAsync();
        }

        #endregion
    }
}
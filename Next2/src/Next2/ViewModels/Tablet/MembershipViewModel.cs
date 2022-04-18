﻿using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Membership;
using Next2.Views.Tablet;
using Prism.Events;
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
        private readonly IEventAggregator _eventAggregator;
        private readonly IPopupNavigation _popupNavigation;

        private MemberModel _member;
        private ObservableCollection<MemberBindableModel> _members;

        public MembershipViewModel(
            IMapper mapper,
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IMembershipService membershipService,
            IPopupNavigation popupNavigation)
            : base(navigationService)
        {
            _mapper = mapper;
            _eventAggregator = eventAggregator;
            _membershipService = membershipService;
            _popupNavigation = popupNavigation;
        }

        #region -- Public properties --

        public ObservableCollection<MemberBindableModel> Members { get; set; } = new ();

        public bool IsMembersRefreshing { get; set; }

        public EMemberSorting MemberSorting { get; set; }

        public string SearchText { get; set; } = string.Empty;

        public string SearchPlaceholder { get; set; }

        public bool IsNotingFound { get; set; } = false;

        public bool IsSearching { get; set; } = false;

        private ICommand _refreshMembersCommand;
        public ICommand RefreshMembersCommand => _refreshMembersCommand ??= new AsyncCommand(OnRefreshMembersCommandAsync, allowsMultipleExecutions: false);

        private ICommand _memberSortingChangeCommand;
        public ICommand MemberSortingChangeCommand => _memberSortingChangeCommand ??= new AsyncCommand<EMemberSorting>(OnMemberSortingChangeCommandAsync, allowsMultipleExecutions: false);

        private ICommand _MembershipEditCommand;
        public ICommand MembershipEditCommand => _MembershipEditCommand ??= new AsyncCommand<MemberBindableModel?>(OnMembershipEditCommandAsync, allowsMultipleExecutions: false);

        private ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new AsyncCommand(OnSearchCommandAsync, allowsMultipleExecutions: false);

        private ICommand _ClearSearchCommand;
        public ICommand ClearSearchCommand => _ClearSearchCommand ??= new AsyncCommand(OnClearSearchCommandAsync);

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
                EMemberSorting.ByCustomerName => x => x.CustomerName,
                _ => throw new NotImplementedException(),
            };

            return members.OrderBy(comparer);
        }

        private async Task RefreshMembersAsync()
        {
            IsMembersRefreshing = true;

            var membersResult = await _membershipService.GetAllMembersAsync();

            if (membersResult.IsSuccess)
            {
                _members = _mapper.Map<ObservableCollection<MemberBindableModel>>(membersResult.Result);

                foreach (var member in _members)
                {
                    member.TapCommand = MembershipEditCommand;
                }

                Members = new(GetSortedMembers(_members));

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
                Members = new(Members.Reverse());
                _members = new(_members.Reverse());
            }
            else
            {
                MemberSorting = memberSorting;

                Members = new(GetSortedMembers(Members));

                _members = new(GetSortedMembers(_members));
            }

            return Task.CompletedTask;
        }

        private async Task OnSearchCommandAsync()
        {
            if (Members.Any() || !string.IsNullOrEmpty(SearchText))
            {
                _eventAggregator.GetEvent<EventSearch>().Subscribe(SearchEventCommand);
                Func<string, string> searchValidator = _membershipService.ApplyNameFilter;
                var parameters = new NavigationParameters()
                {
                    { Constants.Navigations.SEARCH, SearchText },
                    { Constants.Navigations.FUNC, searchValidator },
                    { Constants.Navigations.PLACEHOLDER, Strings.NameOrPhone },
                };
                ClearSearchAsync();
                IsSearching = true;
                await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
            }
        }

        private void SearchEventCommand(string searchLine)
        {
            SearchText = searchLine;

            Members = new(_members.Where(x => x.CustomerName.ToLower().Contains(SearchText.ToLower()) || x.Phone.ToLower().Contains(SearchText.ToLower())));

            _eventAggregator.GetEvent<EventSearch>().Unsubscribe(SearchEventCommand);
        }

        private async Task OnClearSearchCommandAsync()
        {
            if (SearchText != string.Empty)
            {
                ClearSearchAsync();
            }
            else
            {
                await OnSearchCommandAsync();
            }
        }

        private void ClearSearchAsync()
        {
            //CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;
            SearchText = string.Empty;

            Members = new(_members);
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
                    { Constants.DialogParameterKeys.TITLE, Strings.AreYouSure },
                    { Constants.DialogParameterKeys.DESCRIPTION, Strings.MembershipUpdate },
                    { Constants.DialogParameterKeys.CANCEL_BUTTON_TEXT, Strings.Cancel },
                    { Constants.DialogParameterKeys.OK_BUTTON_TEXT, Strings.Ok },
                };

                PopupPage confirmDialog = new ConfirmDialog(confirmDialogParameters, CloseConfirmDialogUpdateCallback);
                await _popupNavigation.PushAsync(confirmDialog);
            }
        }

        private async void CloseConfirmDialogUpdateCallback(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isMembershipDisableAccepted) && isMembershipDisableAccepted)
            {
                await _membershipService.UpdateMemberAsync(_member);

                await RefreshMembersAsync();
            }

            await _popupNavigation.PopAsync();
        }

        #endregion
    }
}
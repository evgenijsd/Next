using AutoMapper;
using Next2.Enums;
using Next2.Helpers.Events;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Services.Membership;
using Next2.Views.Tablet;
using Next2.Views.Tablet.Dialogs;
using Prism.Events;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private MembershipModelDTO _member;
        private List<MemberBindableModel> _allMembers = new();

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

        public ObservableCollection<MemberBindableModel> DisplayMembers { get; set; } = new();

        public bool AnyMembersLoaded { get; set; } = false;

        public bool IsMembersRefreshing { get; set; }

        public EMemberSorting MemberSorting { get; set; }

        public string SearchText { get; set; } = string.Empty;

        private ICommand _refreshMembersCommand;
        public ICommand RefreshMembersCommand => _refreshMembersCommand ??= new AsyncCommand(OnRefreshMembersCommandAsync, allowsMultipleExecutions: false);

        private ICommand _memberSortingChangeCommand;
        public ICommand MemberSortingChangeCommand => _memberSortingChangeCommand ??= new AsyncCommand<EMemberSorting>(OnMemberSortingChangeCommandAsync, allowsMultipleExecutions: false);

        private ICommand _MembershipEditCommand;
        public ICommand MembershipEditCommand => _MembershipEditCommand ??= new AsyncCommand<MemberBindableModel?>(OnMembershipEditCommandAsync, allowsMultipleExecutions: false);

        private ICommand _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new AsyncCommand(OnSearchCommandAsync, allowsMultipleExecutions: false);

        private ICommand _clearSearchCommand;
        public ICommand ClearSearchCommand => _clearSearchCommand ??= new AsyncCommand(OnClearSearchCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName is nameof(DisplayMembers))
            {
                AnyMembersLoaded = _allMembers.Any();
            }
        }

        public override async void OnAppearing()
        {
            base.OnAppearing();

            MemberSorting = EMemberSorting.ByMembershipStartTime;

            await RefreshMembersAsync();
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();

            ClearSearch();
            AnyMembersLoaded = false;
        }

        #endregion

        #region -- Private helpers --

        private ObservableCollection<MemberBindableModel> GetSortedMembers(IEnumerable<MemberBindableModel> members)
        {
            Func<MemberBindableModel, object> comparer = MemberSorting switch
            {
                EMemberSorting.ByMembershipStartTime => x => x.StartDate,
                EMemberSorting.ByMembershipEndTime => x => x.EndDate,
                EMemberSorting.ByCustomerName => x => x.CustomerName,
                _ => throw new NotImplementedException(),
            };

            var result = _mapper.Map<ObservableCollection<MemberBindableModel>>(members.OrderBy(comparer));

            return result;
        }

        private async Task RefreshMembersAsync()
        {
            IsMembersRefreshing = true;

            var membersResult = await _membershipService.GetAllMembersAsync();

            if (membersResult.IsSuccess)
            {
                var result = _mapper.Map<List<MemberBindableModel>>(membersResult.Result);

                foreach (var member in result)
                {
                    member.TapCommand = MembershipEditCommand;
                }

                _allMembers = result;

                DisplayMembers = new(GetSortedMembers(SearchMembers(SearchText)));

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
                DisplayMembers = new(DisplayMembers.Reverse());
            }
            else
            {
                MemberSorting = memberSorting;

                DisplayMembers = new(GetSortedMembers(SearchMembers(SearchText)));
            }

            return Task.CompletedTask;
        }

        private async Task OnSearchCommandAsync()
        {
            if (DisplayMembers.Any() || !string.IsNullOrEmpty(SearchText))
            {
                _eventAggregator.GetEvent<EventSearch>().Subscribe(OnSearchEvent);

                Func<string, string> searchValidator = _membershipService.ApplyNameFilter;
                var parameters = new NavigationParameters()
                {
                    { Constants.Navigations.SEARCH, SearchText },
                    { Constants.Navigations.FUNC, searchValidator },
                    { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["NameOrPhone"] },
                };

                ClearSearch();

                await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
            }
        }

        private void OnSearchEvent(string searchLine)
        {
            SearchText = searchLine;

            DisplayMembers = new(GetSortedMembers(SearchMembers(SearchText)));

            _eventAggregator.GetEvent<EventSearch>().Unsubscribe(OnSearchEvent);
        }

        private List<MemberBindableModel> SearchMembers(string searchLine)
        {
            bool containsName(MemberBindableModel x) => x.CustomerName.ToLower().Contains(searchLine.ToLower());
            bool containsPhone(MemberBindableModel x) => x.Phone.Replace("-", string.Empty).Contains(searchLine);

            return _allMembers.Where(x => containsName(x) || containsPhone(x)).ToList();
        }

        private Task OnClearSearchCommandAsync()
        {
            ClearSearch();

            return Task.CompletedTask;
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;

            DisplayMembers = new(GetSortedMembers(SearchMembers(SearchText)));
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
                _member = _mapper.Map<MemberBindableModel, MembershipModelDTO>(member);

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
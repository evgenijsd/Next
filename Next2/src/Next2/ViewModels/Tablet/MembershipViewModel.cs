using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Models.API.DTO;
using Next2.Services.Membership;
using Next2.Services.Notifications;
using Next2.Views.Tablet;
using Next2.Views.Tablet.Dialogs;
using Prism.Events;
using Prism.Navigation;
using Prism.Services.Dialogs;
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
        private readonly INotificationsService _notificationsService;

        private MembershipModelDTO _member;
        private List<MemberBindableModel> _allMembers = new();

        public MembershipViewModel(
            IMapper mapper,
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            INotificationsService notificationsService,
            IMembershipService membershipService)
            : base(navigationService)
        {
            _mapper = mapper;
            _membershipService = membershipService;
            _notificationsService = notificationsService;
        }

        #region -- Public properties --

        public ObservableCollection<MemberBindableModel> DisplayMembers { get; set; } = new();

        public bool AnyMembersLoaded { get; set; } = false;

        public bool IsMembersRefreshing { get; set; }

        public EMemberSorting MemberSorting { get; set; }

        public string SearchText { get; set; } = string.Empty;

        private ICommand? _refreshMembersCommand;
        public ICommand RefreshMembersCommand => _refreshMembersCommand ??= new AsyncCommand(OnRefreshMembersCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _memberSortingChangeCommand;
        public ICommand MemberSortingChangeCommand => _memberSortingChangeCommand ??= new AsyncCommand<EMemberSorting>(OnMemberSortingChangeCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _membershipEditCommand;
        public ICommand MembershipEditCommand => _membershipEditCommand ??= new AsyncCommand<MemberBindableModel?>(OnMembershipEditCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _searchCommand;
        public ICommand SearchCommand => _searchCommand ??= new AsyncCommand(OnSearchCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _clearSearchCommand;
        public ICommand ClearSearchCommand => _clearSearchCommand ??= new AsyncCommand(OnClearSearchCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case nameof(DisplayMembers):
                    AnyMembersLoaded = _allMembers.Any();
                    break;
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

            SearchText = string.Empty;
            DisplayMembers = new();
        }

        #endregion

        #region -- Public helpers --

        public void SetSearchQuery(string searchMember)
        {
            SearchText = searchMember;

            DisplayMembers = new(GetSortedMembers(SearchMembers(SearchText)));
        }

        #endregion

        #region -- Private helpers --

        private ObservableCollection<MemberBindableModel> GetSortedMembers(IEnumerable<MemberBindableModel> members)
        {
            Func<MemberBindableModel, object> comparer = MemberSorting switch
            {
                EMemberSorting.ByMembershipStartTime => x => x.StartDate,
                EMemberSorting.ByMembershipEndTime => x => x.EndDate,
                EMemberSorting.ByCustomerName => x => x.Customer.FullName,
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

        private Task OnRefreshMembersCommandAsync()
        {
            return RefreshMembersAsync();
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
                Func<string, string> searchValidator = Filters.StripInvalidNameCharacters;

                var parameters = new NavigationParameters()
                {
                    { Constants.Navigations.SEARCH_MEMBER, SearchText },
                    { Constants.Navigations.FUNC, searchValidator },
                    { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["NameOrPhone"] },
                };

                await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
            }
        }

        private List<MemberBindableModel> SearchMembers(string searchText)
        {
            bool containsName(MemberBindableModel x) => x.Customer.FullName.Contains(searchText, StringComparison.OrdinalIgnoreCase);
            bool containsPhone(MemberBindableModel x) => x.Customer.Phone.Contains(searchText);

            return _allMembers.Where(x => containsName(x) || containsPhone(x)).ToList();
        }

        private Task OnClearSearchCommandAsync()
        {
            SetSearchQuery(string.Empty);

            return Task.CompletedTask;
        }

        private async Task OnMembershipEditCommandAsync(MemberBindableModel? member)
        {
            if (member is MemberBindableModel selectedMember)
            {
                var parameters = new DialogParameters { { Constants.DialogParameterKeys.MODEL, selectedMember } };

                PopupPage popupPage = new Views.Tablet.Dialogs.MembershipEditDialog(parameters, MembershipEditDialogCallBack, _mapper);

                await PopupNavigation.PushAsync(popupPage);
            }
        }

        private async void MembershipEditDialogCallBack(IDialogParameters parameters)
        {
            await _notificationsService.CloseAllPopupAsync();

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
                await PopupNavigation.PushAsync(confirmDialog);
            }
        }

        private async void CloseConfirmDialogUpdateCallback(IDialogParameters parameters)
        {
            if (parameters.TryGetValue(Constants.DialogParameterKeys.ACCEPT, out bool isMembershipDisableAccepted) && isMembershipDisableAccepted)
            {
                await _membershipService.UpdateMemberAsync(_member);
                await RefreshMembersAsync();
            }

            await _notificationsService.CloseAllPopupAsync();
        }

        #endregion
    }
}
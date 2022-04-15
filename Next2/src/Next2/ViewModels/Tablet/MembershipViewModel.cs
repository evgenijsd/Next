using AutoMapper;
using Next2.Enums;
using Next2.Helpers;
using Next2.Models;
using Next2.Services.Membership;
using Next2.Views.Tablet;
using Prism.Events;
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
        private readonly IMapper _mapper;
        private readonly IMembershipService _membershipService;
        private readonly IEventAggregator _eventAggregator;

        public MembershipViewModel(
            IMapper mapper,
            INavigationService navigationService,
            IEventAggregator eventAggregator,
            IMembershipService membershipService)
            : base(navigationService)
        {
            _mapper = mapper;
            _eventAggregator = eventAggregator;
            _membershipService = membershipService;
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
        public ICommand RefreshMembersCommand => _refreshMembersCommand ??= new AsyncCommand(OnRefreshMembersCommandAsync);

        private ICommand _memberSortingChangeCommand;
        public ICommand MemberSortingChangeCommand => _memberSortingChangeCommand ??= new AsyncCommand<EMemberSorting>(OnMemberSortingChangeCommandAsync);

        private ICommand _SearchCommand;
        public ICommand SearchCommand => _SearchCommand ??= new AsyncCommand(OnSearchCommandAsync, allowsMultipleExecutions: false);

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
                };
                ClearSearchAsync();
                IsSearching = true;
                await _navigationService.NavigateAsync(nameof(SearchPage), parameters);
            }
        }

        private void SearchEventCommand(string searchLine)
        {
            SearchText = searchLine;

            /*Orders = new(Orders.Where(x => x.OrderNumberText.ToLower().Contains(SearchText.ToLower()) || x.Name.ToLower().Contains(SearchText.ToLower())));
            SelectedOrder = null;

            _eventAggregator.GetEvent<EventSearch>().Unsubscribe(SearchEventCommand);

            SetHeightCollection();*/
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
            /*CurrentOrderTabSorting = EOrderTabSorting.ByCustomerName;

            SelectedOrder = null;
            SearchText = string.Empty;

            SetVisualCollection();*/
        }

        #endregion
    }
}
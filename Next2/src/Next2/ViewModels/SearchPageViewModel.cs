using Next2.Enums;
using Prism.Navigation;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class SearchPageViewModel : BaseViewModel
    {
        private ESearchType _searchType;

        public SearchPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        public int CursorPosition { get; set; }

        public bool IsOrderTabsSelected { get; set; } = true;

        public Func<string, string> ApplySearchFilter;

        public string SearchLine { get; set; } = string.Empty;

        public string Placeholder { get; set; } = string.Empty;

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand<string>(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.FUNC, out Func<string, string> searchValidator))
            {
                ApplySearchFilter = searchValidator;
            }

            if (parameters.TryGetValue(Constants.Navigations.SEARCH, out string searchLine))
            {
                SearchLine = searchLine ?? string.Empty;
                CursorPosition = SearchLine.Length;
            }

            if (parameters.TryGetValue(Constants.Navigations.SEARCH_MEMBER, out string searchMember))
            {
                SearchLine = searchMember ?? string.Empty;
                CursorPosition = SearchLine.Length;
                _searchType = ESearchType.Member;
            }

            if (parameters.TryGetValue(Constants.Navigations.SEARCH_CUSTOMER, out string searchCustomer))
            {
                SearchLine = searchCustomer ?? string.Empty;
                CursorPosition = SearchLine.Length;
                _searchType = ESearchType.Customer;
            }

            if (parameters.TryGetValue(Constants.Navigations.PLACEHOLDER, out string placeholder))
            {
                Placeholder = placeholder;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SearchLine))
            {
                SearchLine = ApplySearchFilter(SearchLine);
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnGoBackCommandAsync(string? done)
        {
            var searchQuery = done ?? SearchLine;
            string constantNavigation;

            if (_searchType == ESearchType.Member)
            {
                constantNavigation = Constants.Navigations.SEARCH_MEMBER;
            }
            else if (_searchType == ESearchType.Customer)
            {
                constantNavigation = Constants.Navigations.SEARCH_CUSTOMER;
            }
            else
            {
                constantNavigation = Constants.Navigations.SEARCH_QUERY;
            }

            var parameters = new NavigationParameters
            {
                { constantNavigation, searchQuery },
            };

            return _navigationService.GoBackAsync(parameters);
        }

        #endregion
    }
}

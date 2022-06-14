using Next2.Helpers.Events;
using Prism.Events;
using Prism.Navigation;
using Rg.Plugins.Popup.Contracts;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class SearchPageViewModel : BaseViewModel
    {
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

            var parameters = new NavigationParameters
            {
                { Constants.Navigations.SEARCH_QUERY, searchQuery },
            };

            return _navigationService.GoBackAsync(parameters);
        }

        #endregion
    }
}

using Next2.Helpers;
using Prism.Events;
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
        private readonly IEventAggregator _eventAggregator;

        public SearchPageViewModel(
            INavigationService navigationService,
            IEventAggregator eventAggregator)
            : base(navigationService)
        {
            _eventAggregator = eventAggregator;
        }

        #region -- Public properties --

        public bool IsOrderTabsSelected { get; set; } = true;

        public Func<string, string> ApplySearchFilter;

        public string SearchLine { get; set; } = string.Empty;

        public string Placeholder { get; set; } = string.Empty;

        private ICommand _GoBackCommand;
        public ICommand GoBackCommand => _GoBackCommand ??= new AsyncCommand<string>(OnGoBackCommandAsync, allowsMultipleExecutions: false);

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

        private async Task OnGoBackCommandAsync(string? done)
        {
            var result = done ?? string.Empty;
            _eventAggregator.GetEvent<EventSearch>().Publish(result);

            await _navigationService.GoBackAsync();
        }

        #endregion
    }
}

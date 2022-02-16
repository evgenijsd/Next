using Next2.Helpers;
using Next2.Models;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        public bool IsOrderTabsSelected { get; set; } = true;

        public string? SearchLine { get; set; } = string.Empty;

        private ICommand _GoBackCommand;
        public ICommand GoBackCommand => _GoBackCommand ??= new AsyncCommand<string>(OnGoBackCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.SEARCH, out SearchParameters inParameter))
            {
                IsOrderTabsSelected = inParameter.IsSelected;
                SearchLine = inParameter.SearchLine;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnGoBackCommandAsync(string? done)
        {
            var parameters = new NavigationParameters { { Constants.Navigations.SEARCH, done } };

            if (!string.IsNullOrEmpty(done))
            {
                MessagingCenter.Send<MessageEvent>(new MessageEvent(done), MessageEvent.SearchMessage);
            }

            await _navigationService.GoBackAsync(parameters);
        }

        #endregion
    }
}

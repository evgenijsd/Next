using Next2.Helpers;
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

        private string _searchLine = string.Empty;

        public string SearchLine
        {
            get => _searchLine;
            set => SetProperty(ref _searchLine, value);
        }

        private ICommand _GoBackCommand;
        public ICommand GoBackCommand => _GoBackCommand ??= new AsyncCommand<string>(OnGoBackCommandAsync);

        #endregion

        #region -- Private helpers --

        private async Task OnGoBackCommandAsync(string done)
        {
            var parametrs = new NavigationParameters { { Constants.Navigations.SEARCH, done } };

            if (!string.IsNullOrEmpty(done))
            {
                MessagingCenter.Send<MessageEvent>(new MessageEvent(done), MessageEvent.SearchMessage);
            }

            await _navigationService.GoBackAsync(parametrs);
        }

        #endregion
    }
}

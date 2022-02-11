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

        private ICommand _GoBackCommand;
        public ICommand GoBackCommand => _GoBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        #endregion

        #region -- Private helpers --

        private async Task OnGoBackCommandAsync()
        {
            MessagingCenter.Send<MessageEvent>(new MessageEvent("SEND"), MessageEvent.SearchMessage);
            await _navigationService.GoBackAsync();
        }

        #endregion
    }
}

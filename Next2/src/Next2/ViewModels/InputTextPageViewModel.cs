using Next2.Models;
using Prism.Navigation;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using static Next2.Constants;

namespace Next2.ViewModels
{
    public class InputTextPageViewModel : BaseViewModel
    {
        public InputTextPageViewModel(INavigationService navigationService)
          : base(navigationService)
        {
        }

        #region -- Public properties--

        public string Text { get; set; }

        public string Placeholder { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Navigations.INPUT_TEXT, out string text))
            {
                Text = text;
            }

            if (parameters.TryGetValue(Navigations.PLACEHOLDER, out string placeholder))
            {
                Placeholder = placeholder;
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnGoBackCommandAsync()
        {
            var param = new NavigationParameters
            {
                { nameof(Navigations.INPUT_TEXT), Text },
            };

            await _navigationService.GoBackAsync(param);
        }

        #endregion
    }
}
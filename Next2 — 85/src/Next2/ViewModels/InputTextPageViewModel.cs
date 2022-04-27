using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using static Next2.Constants;

namespace Next2.ViewModels
{
    public class InputTextPageViewModel : BaseViewModel
    {
        private string _oldText;

        public InputTextPageViewModel(INavigationService navigationService)
          : base(navigationService)
        {
        }

        #region -- Public properties--

        public string Text { get; set; }

        public string Placeholder { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        private ICommand _returnCommand;
        public ICommand ReturnCommand => _returnCommand ??= new AsyncCommand(OnReturnCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Navigations.INPUT_TEXT, out string text))
            {
                Text = text;
                _oldText = text;
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
                { nameof(Navigations.INPUT_TEXT), _oldText },
            };

            await _navigationService.GoBackAsync(param);
        }

        private async Task OnReturnCommandAsync()
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
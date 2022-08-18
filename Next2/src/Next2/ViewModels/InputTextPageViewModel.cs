using Next2.Services.Authentication;
using Next2.Services.Notifications;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using static Next2.Constants;

namespace Next2.ViewModels
{
    public class InputTextPageViewModel : BaseViewModel
    {
        private string? _oldText;

        public InputTextPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService)
          : base(navigationService, authenticationService, notificationsService)
        {
        }

        #region -- Public properties --

        public string Text { get; set; } = string.Empty;

        public string Placeholder { get; set; } = string.Empty;

        private ICommand? _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand ??= new AsyncCommand(OnGoBackCommandAsync);

        private ICommand? _returnCommand;
        public ICommand ReturnCommand => _returnCommand ??= new AsyncCommand(OnReturnCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.TryGetValue(Navigations.INPUT_VALUE, out string text))
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

        private Task OnGoBackCommandAsync()
        {
            var param = new NavigationParameters
            {
                { nameof(Navigations.INPUT_VALUE), _oldText },
            };

            return _navigationService.GoBackAsync(param);
        }

        private Task OnReturnCommandAsync()
        {
            var param = new NavigationParameters
            {
                { nameof(Navigations.INPUT_VALUE), Text },
            };

            return _navigationService.GoBackAsync(param);
        }

        #endregion
    }
}
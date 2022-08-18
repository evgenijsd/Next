using Next2.Services.Authentication;
using Next2.Services.Notifications;
using Next2.Views.Mobile;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Mobile
{
    public class WaitingSwipeCardPageViewModel : BaseViewModel
    {
        public WaitingSwipeCardPageViewModel(
            INavigationService navigationService,
            IAuthenticationService authenticationService,
            INotificationsService notificationsService)
            : base(navigationService, authenticationService, notificationsService)
        {
        }

        #region -- Public properties --

        public bool NeedSignatureReceipt { get; set; }

        private ICommand? _changeCardPaymentStatusCommand;
        public ICommand ChangeCardPaymentStatusCommand => _changeCardPaymentStatusCommand ??= new AsyncCommand(OnChangeCardPaymentStatusCommandAsync, allowsMultipleExecutions: false);

        private ICommand? _tapCheckBoxSignatureReceiptCommand;
        public ICommand TapCheckBoxSignatureReceiptCommand => _tapCheckBoxSignatureReceiptCommand ??= new Command(() => NeedSignatureReceipt = !NeedSignatureReceipt);

        #endregion

        #region -- Overrides --

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey(Constants.Navigations.PAYMENT_COMPLETE))
            {
                await _navigationService.GoBackAsync(parameters);
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnChangeCardPaymentStatusCommandAsync()
        {
            if (NeedSignatureReceipt)
            {
                var navigationParam = new NavigationParameters()
                {
                    { Constants.Navigations.PAYMENT_COMPLETE, string.Empty },
                };

                await _navigationService.GoBackAsync(navigationParam);
            }
            else
            {
                await _navigationService.NavigateAsync(nameof(WaitingSignaturePage));
            }
        }

        #endregion
    }
}

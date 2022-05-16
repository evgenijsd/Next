using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels.Mobile
{
    public class WaitingSignaturePageViewModel : BaseViewModel
    {
        public WaitingSignaturePageViewModel(
            INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        public byte[] BitmapSignature { get; set; }

        public bool IsCleared { get; set; } = true;

        private ICommand _clearDrawPanelCommand;
        public ICommand ClearDrawPanelCommand => _clearDrawPanelCommand ??= new Command(() => IsCleared = true);

        private ICommand _tapPaymentCompleteCommand;
        public ICommand TapPaymentCompleteCommand => _tapPaymentCompleteCommand ??= new AsyncCommand(OnTapPaymentCompleteCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private methods --

        private async Task OnTapPaymentCompleteCommandAsync()
        {
            var navigationParam = new NavigationParameters()
            {
                { Constants.Navigations.PAYMENT_COMPLETE, string.Empty },
                { Constants.Navigations.SIGNATURE, BitmapSignature },
            };

            await _navigationService.GoBackAsync(navigationParam);
        }

        #endregion
    }
}

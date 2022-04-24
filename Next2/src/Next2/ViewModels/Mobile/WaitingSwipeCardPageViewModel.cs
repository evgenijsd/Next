using Next2.Views.Mobile;
using Prism.Navigation;
using Prism.Services.Dialogs;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class WaitingSwipeCardPageViewModel : BaseViewModel
    {
        public WaitingSwipeCardPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        public bool NeedSignatureReceipt { get; set; }

        private ICommand _changeCardPaymentStatusCommand;
        public ICommand ChangeCardPaymentStatusCommand => _changeCardPaymentStatusCommand ??= new AsyncCommand(OnChangeCardPaymentStatusCommandAsync, allowsMultipleExecutions: false);

        private ICommand _tapCheckBoxSignatureReceiptCommand;
        public ICommand TapCheckBoxSignatureReceiptCommand => _tapCheckBoxSignatureReceiptCommand ??= new AsyncCommand(OnTapCheckBoxSignatureReceiptCommandAsync, allowsMultipleExecutions: false);

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

        private Task OnTapCheckBoxSignatureReceiptCommandAsync()
        {
            NeedSignatureReceipt = !NeedSignatureReceipt;
            return Task.CompletedTask;
        }

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

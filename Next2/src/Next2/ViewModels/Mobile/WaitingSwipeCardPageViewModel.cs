using Next2.Views.Mobile;
using Prism.Navigation;
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

        private ICommand _changeCardPaymentStatusCommand;
        public ICommand ChangeCardPaymentStatusCommand => _changeCardPaymentStatusCommand ??= new AsyncCommand(OnChangeCardPaymentStatusCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnChangeCardPaymentStatusCommandAsync()
        {
            return _navigationService.NavigateAsync(nameof(WaitingSignaturePage));
        }

        #endregion
    }
}

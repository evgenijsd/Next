using Next2.Views.Tablet;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class PaymentCompleteViewModel : BaseViewModel
    {
        public PaymentCompleteViewModel(
            INavigationService navigationService)
            : base(navigationService)
        {
        }

        private ICommand _OpenTipsCommand;
        public ICommand OpenTipsCommand => _OpenTipsCommand ??= new AsyncCommand(OnOpenTipsCommandAsync, allowsMultipleExecutions: false);

        private async Task OnOpenTipsCommandAsync()
        {
            await _navigationService.NavigateAsync(nameof(TipsPage));
        }
    }
}

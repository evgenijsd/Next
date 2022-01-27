using Prism.Navigation;

namespace Next2.ViewModels
{
    public class ReservationsViewModel : BaseViewModel
    {
        public ReservationsViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "Reservations";
        }

        public string? Text { get; set; }
    }
}

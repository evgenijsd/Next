using Prism.Navigation;

namespace Next2.ViewModels
{
    public class CustomersViewModel : BaseViewModel
    {
        public CustomersViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "Customers";
        }

        public string? Text { get; set; }
    }
}
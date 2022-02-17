using Next2.Helpers;
using Next2.Models;
using Next2.Services.OrderService;
using Prism.Navigation;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        public SearchViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
        }

        #region -- Public properties --

        public bool IsOrderTabsSelected { get; set; } = true;

        public string? SearchLine { get; set; } = string.Empty;

        private ICommand _GoBackCommand;
        public ICommand GoBackCommand => _GoBackCommand ??= new AsyncCommand<string>(OnGoBackCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.SEARCH, out SearchParameters inParameter))
            {
                IsOrderTabsSelected = inParameter.IsSelected;
                SearchLine = inParameter.SearchLine;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(SearchLine))
            {
                SearchLine = _orderService.SearchValidator(IsOrderTabsSelected, SearchLine);
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnGoBackCommandAsync(string? done)
        {
            var parameters = new NavigationParameters { { Constants.Navigations.SEARCH, done } };

            if (!string.IsNullOrEmpty(done))
            {
                MessagingCenter.Send<MessageEvent>(new MessageEvent(done), MessageEvent.SearchMessage);
            }

            await _navigationService.GoBackAsync(parameters);
        }

        #endregion
    }
}

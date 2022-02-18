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
    public class SearchPageViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        public SearchPageViewModel(
            INavigationService navigationService,
            IOrderService orderService)
            : base(navigationService)
        {
            _orderService = orderService;
        }

        #region -- Public properties --

        public bool IsOrderTabsSelected { get; set; } = true;

        public string SearchLine { get; set; } = string.Empty;

        private ICommand _GoBackCommand;
        public ICommand GoBackCommand => _GoBackCommand ??= new AsyncCommand<string>(OnGoBackCommandAsync);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.SEARCH, out SearchParameters inParameter))
            {
                IsOrderTabsSelected = inParameter.IsSelected;
                SearchLine = inParameter.SearchLine ?? string.Empty;
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
            var result = done ?? string.Empty;
            MessagingCenter.Send(new MessageEvent(result), MessageEvent.SearchMessage);

            await _navigationService.GoBackAsync();
        }

        #endregion
    }
}

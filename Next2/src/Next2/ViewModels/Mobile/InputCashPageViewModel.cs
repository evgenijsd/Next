using Prism.Navigation;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels.Mobile
{
    public class InputCashPageViewModel : BaseViewModel
    {
        public InputCashPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        #region -- Public properties --

        public float Total { get; set; }

        public float Change { get; set; }

        public string InputValue { get; set; }

        private ICommand _goBackCommand;
        public ICommand GoBackCommand => _goBackCommand = new AsyncCommand(OnGoBackCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.TryGetValue(Constants.Navigations.TOTAL_SUM, out float sum))
            {
                Total = sum;
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            if (args.PropertyName == nameof(InputValue))
            {
                if (float.TryParse(InputValue, out float value))
                {
                    value /= 100;
                    Change = value > Total ? value - Total : 0;
                }
                else
                {
                    Change = 0;
                }
            }
        }

        #endregion

        #region -- Private methods --

        private Task OnGoBackCommandAsync()
        {
            var navigationParam = new NavigationParameters()
            {
                { Constants.Navigations.INPUT_VALUE, InputValue },
            };

            return _navigationService.GoBackAsync(navigationParam);
        }

        #endregion
    }
}

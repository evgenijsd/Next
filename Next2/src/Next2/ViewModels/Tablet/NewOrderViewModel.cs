using Next2.Interfaces;
using Next2.Services.MockService;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.ViewModels.Tablet
{
    public class NewOrderViewModel : BaseViewModel, IPageActionsHandler
    {
        private IMockService _mockService;

        public NewOrderViewModel(
            INavigationService navigationService,
            IMockService mockService)
            : base(navigationService)
        {
            _mockService = mockService;
            Text = "NewOrder";
        }

        #region -- Public properties --

        public string? Text { get; set; }

        #endregion

        #region -- IPageActionsHandler implementation --

        public void OnAppearing()
        {
        }

        public void OnDisappearing()
        {
        }

        #endregion
    }
}
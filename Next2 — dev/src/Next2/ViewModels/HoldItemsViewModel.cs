using Next2.Views.Mobile;
using Next2.Views.Tablet;
using Prism.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.ViewModels
{
    public class HoldItemsViewModel : BaseViewModel
    {
        public HoldItemsViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "HoldItems";
        }

        #region -- Public properties --

        public string? Text { get; set; }

        #endregion
    }
}

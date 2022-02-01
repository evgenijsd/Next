using Prism.Navigation;

namespace Next2.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Text = "Settings";
        }

        #region -- Public properties --

        public string? Text { get; set; }

        #endregion

        #region -- IPageActionsHandler implementation --

        public override void OnAppearing()
        {
        }

        public override void OnDisappearing()
        {
        }

        #endregion
    }
}

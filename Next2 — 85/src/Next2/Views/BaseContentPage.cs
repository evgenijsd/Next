using Next2.Helpers;
using Next2.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Next2.Views
{
    public class BaseContentPage : ContentPage
    {
        public BaseContentPage()
        {
            On<iOS>().SetUseSafeArea(true);
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
        }

        #region -- Overrides --

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is IPageActionsHandler actionsHandler)
            {
                actionsHandler.OnAppearing();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BindingContext is IPageActionsHandler actionsHandler)
            {
                actionsHandler.OnDisappearing();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        #endregion
    }
}

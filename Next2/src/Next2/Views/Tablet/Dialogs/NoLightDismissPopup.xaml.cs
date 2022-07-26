using Prism.Navigation;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Next2.Views.Tablet.Dialogs
{
    public partial class NoLightDismissPopup : Popup
    {
        public NoLightDismissPopup() => InitializeComponent();

        private INavigation Navigation2 => App.Current.MainPage.Navigation;

        private void Button_Clicked(object? sender, System.EventArgs e) => Dismiss("Dismiss was clicked");

        private void Button_Clicked_1(object sender, System.EventArgs e)
        {
            var nav = App.Resolve<INavigationService>();

            var navigationParameters = new NavigationParameters()
            {
                { Constants.Navigations.INPUT_NOTES, "Notes" },
                { Constants.Navigations.PLACEHOLDER, LocalizationResourceManager.Current["CommentForReservation"] },
            };

            var page = new ContentPage()
            {
                Content = new StackLayout()
                {
                    BackgroundColor = Color.Red,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = 500,
                    WidthRequest = 500,
                    Children =
                    {
                        new Label()
                        {
                            Text = "sddssd",
                        },
                    },
                },
            };

            nav.NavigateAsync(nameof(SearchPage), navigationParameters, true);
            Navigation.PushAsync(page);
        }
    }
}
using Next2.Services.CustomersService;
using Next2.ViewModels;
using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace Next2.Views.Tablet
{
    public partial class CustomersView : ContentView
    {
        public CustomersView()
        {
            InitializeComponent();
        }

        private void ItemTappedHandler(object sender, EventArgs args)
        {
            if (collectionView.SelectedItem == null)
            {
                selectButton.Opacity = 0.32;
                infoButton.Opacity = 0.32;
            }
            else
            {
                selectButton.Opacity = 1;
                infoButton.Opacity = 1;
            }
        }
    }
}
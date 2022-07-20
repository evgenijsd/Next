using Next2.Interfaces;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

namespace Next2.Views.Tablet
{
    public partial class PrintReceiptView : ContentView
    {
        private bool _viewLoaded = false;
        public PrintReceiptView()
        {
            InitializeComponent();
        }

        #region -- Overrides --

        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);
            if (propertyName == "Renderer")
            {
                if (BindingContext is IPageActionsHandler handler)
                {
                    if (_viewLoaded)
                    {
                        handler.OnDisappearing();
                    }
                    else
                    {
                        handler.OnAppearing();
                    }

                    _viewLoaded = !_viewLoaded;
                }
            }
        }

        #endregion
    }
}
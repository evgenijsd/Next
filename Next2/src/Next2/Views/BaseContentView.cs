using Next2.Interfaces;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Views
{
    public class BaseContentView : ContentView
    {
        private bool _isViewLoaded = false;

        public BaseContentView()
        {
        }

        #region -- Overrides --

        protected override void OnPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanging(propertyName);

            if (propertyName == "Renderer")
            {
                if (BindingContext is IPageActionsHandler handler)
                {
                    if (_isViewLoaded)
                    {
                        handler.OnDisappearing();
                    }
                    else
                    {
                        handler.OnAppearing();
                    }

                    _isViewLoaded = !_isViewLoaded;
                }
            }
        }

        #endregion
    }
}

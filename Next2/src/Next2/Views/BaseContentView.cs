using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Next2.Views
{
    public class BaseContentView : ContentView
    {
        private bool _viewLoaded = false;
        public BaseContentView()
        {
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

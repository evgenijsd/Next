using System;
using System.Collections.Generic;
using System.Text;

namespace Next2.Helpers
{
    public interface IPageActionsHandler
    {
        void OnAppearing();
        void OnDisappearing();
    }
}

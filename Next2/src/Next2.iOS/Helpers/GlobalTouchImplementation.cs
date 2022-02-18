using Foundation;
using Next2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;

namespace Next2.iOS.Helpers
{
    public class GlobalTouchImplementation : IGlobalTouch
    {
        EventHandler globalTouchHandler;

        public void TapScreen(EventHandler handler)
        {
            globalTouchHandler = handler;
        }

        public void TapScreen()
        {
            globalTouchHandler?.Invoke(this, null);
        }
    }
}
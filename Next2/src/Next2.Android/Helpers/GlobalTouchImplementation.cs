using Next2.Droid.Next2.Droid;
using Next2.Helpers;
using System;

namespace Next2.Droid.Helpers
{
    public class GlobalTouchImplementation : IGlobalTouch
    {
        public void DetachTapScreen(EventHandler handler)
        {
            (MainApplication.CurrentContext as MainActivity).GlobalTouchHandler -= handler;
        }

        public void TapScreen(EventHandler handler)
        {
            (MainApplication.CurrentContext as MainActivity).GlobalTouchHandler += handler;
        }

        public void TapScreen()
        {

        }
    }
}
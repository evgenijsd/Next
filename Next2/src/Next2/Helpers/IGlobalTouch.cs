using System;

namespace Next2.Helpers
{
    public interface IGlobalTouch
    {
        void TapScreen(EventHandler handler);

        void DetachTapScreen(EventHandler handler);

        void TapScreen();
    }
}

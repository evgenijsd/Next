using Xamarin.Forms;

namespace Next2.Effects
{
    public class TouchEffect : RoutingEffect
    {
        public TouchEffect()
            : base("com.headworks.TouchEffect")
        {
        }

        #region -- Public properties --

        public event TouchActionEventHandler? TouchAction;

        public bool Capture { get; set; }

        #endregion

        #region -- Public helpers --

        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }

        #endregion
    }
}

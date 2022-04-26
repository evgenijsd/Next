using Xamarin.Forms;

namespace Next2.Effects
{
    public class TouchEffect : RoutingEffect
    {
        public event TouchActionEventHandler TouchAction;

        public TouchEffect()
            : base($"Next2.Effects.{nameof(TouchEffect)}")
        {
        }

        public bool Capture { get; set; }

        public void OnTouchAction(Element element, TouchActionEventArgs args)
        {
            TouchAction?.Invoke(element, args);
        }
    }
}

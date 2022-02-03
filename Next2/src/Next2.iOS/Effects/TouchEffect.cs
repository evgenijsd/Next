using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportEffect(typeof(Next2.Effects.iOS.TouchEffect), "TouchEffect")]

namespace Next2.Effects.iOS
{
    public class TouchEffect : PlatformEffect
    {
        private UIView view;
        private TouchRecognizer touchRecognizer;

        protected override void OnAttached()
        {
            view = Control ?? Container;

            var effect = (Next2.Effects.TouchEffect)Element.Effects.FirstOrDefault(e => e is Next2.Effects.TouchEffect);

            if (effect != null && view != null)
            {
                touchRecognizer = new TouchRecognizer(Element, view, effect); 
                view.AddGestureRecognizer(touchRecognizer);
            }
        }

        protected override void OnDetached()
        {
            if (touchRecognizer != null)
            {
                touchRecognizer.Detach();

                view.RemoveGestureRecognizer(touchRecognizer);
            }
        }
    }
}
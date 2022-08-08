using Next2.iOS.Effects;
using Next2.iOS.Helpers;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(TouchEffect), nameof(TouchEffect))]
namespace Next2.iOS.Effects
{
    public class TouchEffect : PlatformEffect
    {
        private UIView _view;
        private TouchRecognizer _touchRecognizer;

        #region -- Overrides --

        protected override void OnAttached()
        {
            _view = Control == null ? Container : Control;

            Next2.Effects.TouchEffect effect = (Next2.Effects.TouchEffect)Element.Effects.FirstOrDefault(e => e is Next2.Effects.TouchEffect);

            if (effect != null && _view != null)
            {
                _touchRecognizer = new TouchRecognizer(Element, _view, effect);
                _view.AddGestureRecognizer(_touchRecognizer);
            }
        }

        protected override void OnDetached()
        {
            if (_touchRecognizer != null)
            {
                _touchRecognizer.Detach();

                _view.RemoveGestureRecognizer(_touchRecognizer);
            }
        }

        #endregion
    }
}
using Next2.iOS.Effects;
using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIEffects = Next2.Effects;

[assembly: ExportEffect(typeof(ScrollBarColorEffect), nameof(ScrollBarColorEffect))]
namespace Next2.iOS.Effects
{
    public class ScrollBarColorEffect : PlatformEffect
    {
        private UIScrollView _view;

        private int _scrollBarCornerRadius = 9;

        private UIColor _scrollBarThumbColor = Color.FromHex("#F45E49").ToUIColor();

        #region -- Overrides --

        protected override void OnAttached()
        {
            Initialize();
        }

        protected override void OnDetached()
        {
            Uninitialize();
        }

        #endregion

        #region -- Private helpers --

        private void Initialize()
        {
            try
            {
                _view = (UIScrollView)(Control ?? Container);
                _view.Scrolled += Container_Scrolled;

                var effect = (UIEffects.ScrollBarColorEffect)Element.Effects.FirstOrDefault(e => e is UIEffects.ScrollBarColorEffect);

                if (effect != null)
                {
                    _scrollBarCornerRadius = effect.ScrollBarCornerRadius;
                    _scrollBarThumbColor = effect.ScrollBarThumbColor.ToUIColor();
                }
            }
            catch
            {
            }
        }

        private void Uninitialize()
        {
            try
            {
                _view.Scrolled -= Container_Scrolled;
            }
            catch(Exception)
            {
            }
        }

        private void Container_Scrolled(object sender, System.EventArgs e)
        {
            try
            {
                var subViews = _view.Subviews.ToList();
                var verticalIndicator = subViews.LastOrDefault();

                if (verticalIndicator != null)
                {
                    verticalIndicator.Layer.CornerRadius = _scrollBarCornerRadius;
                    verticalIndicator.BackgroundColor = _scrollBarThumbColor;
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}
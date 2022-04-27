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
        UIScrollView _view;

        int _scrollBarCornerRadius = 9;

        UIColor _scrollBarThumbColor = Color.FromHex("#F45E49").ToUIColor();

        protected override void OnAttached()
        {
            Initialize();
        }

        protected override void OnDetached()
        {
            Uninitialize();
        }

        void Initialize()
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

        void Uninitialize()
        {
            try
            {
                _view.Scrolled -= Container_Scrolled;
            }
            catch(Exception ex)
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
    }
}
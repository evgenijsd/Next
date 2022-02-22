using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Google.Android.Material.Shape;
using Java.Lang;
using Next2.Droid.Effects;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using UIEffects = Next2.Effects;

[assembly: ResolutionGroupName("Next2.Effects")]
[assembly: ExportEffect(typeof(ScrollBarColorEffect), nameof(ScrollBarColorEffect))]

namespace Next2.Droid.Effects
{
    public class ScrollBarColorEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            UpdateUI();
        }

        protected override void OnDetached()
        {
        }

        void UpdateUI()
        {
            var scrollBarThumbWidth = 6;
            var scrollBarCornerRadius = 9;
            var scrollBarThumbColor = Color.FromHex("#F45E49").ToAndroid();
            var scrollBarTrackColor = Color.FromHex("#424861").ToAndroid();

            var effect = (UIEffects.ScrollBarColorEffect)Element.Effects.FirstOrDefault(e => e is UIEffects.ScrollBarColorEffect);
            if (effect != null)
            {
                scrollBarThumbWidth = effect.ScrollBarThumbWidth;
                scrollBarCornerRadius = effect.ScrollBarCornerRadius;
                scrollBarThumbColor = effect.ScrollBarThumbColor.ToAndroid();
                scrollBarTrackColor = effect.ScrollBarTrackColor.ToAndroid();
            }

            var drawableThumb = this.GetGradientDrawable(scrollBarThumbColor, scrollBarCornerRadius);
            var drawableTrack = this.GetGradientDrawable(scrollBarTrackColor, scrollBarCornerRadius);

            try
            {
                Java.Lang.Reflect.Field mScrollCacheField = Class.FromType(typeof(Android.Views.View)).GetDeclaredField("mScrollCache");
                mScrollCacheField.Accessible = true;

                var mScrollCache = mScrollCacheField.Get(Control);
                var scrollBarField = mScrollCache.Class.GetDeclaredField("scrollBar");
                scrollBarField.Accessible = true;

                var scrollBar = scrollBarField.Get(mScrollCache);

                var method = scrollBar.Class.GetDeclaredMethod("setVerticalThumbDrawable", Class.FromType(typeof(Drawable)));
                method.Accessible = true;

                var layers = new Drawable[1];

                layers[0] = drawableThumb;

                method.Invoke(scrollBar, layers);
            }
            catch
            {
                try
                {
                    Control.VerticalScrollbarThumbDrawable = drawableThumb;
                    Control.VerticalScrollbarTrackDrawable = drawableTrack;
                }
                catch
                {
                }
            }
        }

        #region -- Private helpers --

        private GradientDrawable GetGradientDrawable(Android.Graphics.Color color, float cornerRadius)
        {
            GradientDrawable gradient = new GradientDrawable();
            gradient.SetCornerRadius(cornerRadius);
            gradient.SetColorFilter(color, Android.Graphics.PorterDuff.Mode.SrcIn);

            return gradient;
        }

        #endregion
    }
}
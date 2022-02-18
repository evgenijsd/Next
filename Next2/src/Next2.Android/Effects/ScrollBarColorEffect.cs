using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
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
            try
            {
                Java.Lang.Reflect.Field mScrollCacheField = Class.FromType(typeof(Android.Views.View)).GetDeclaredField("mScrollCache");
                mScrollCacheField.Accessible = true;

                var mScrollCache = mScrollCacheField.Get(Control);
                var scrollBarField = mScrollCache.Class.GetDeclaredField("scrollBar");
                scrollBarField.Accessible = true;

                var scrollBar = scrollBarField.Get(mScrollCache);

                if (Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.Lollipop)
                {
                }
                else
                {
                }

                var layers = new Drawable[1];
                var shapeDrawable = new ShapeDrawable(new RectShape());
                var scrollBarColor = Color.Default;

                var effect = (UIEffects.ScrollBarColorEffect)Element.Effects.FirstOrDefault(e => e is UIEffects.ScrollBarColorEffect);
                if (effect != null)
                {
                    scrollBarColor = effect.ScrollBarColor;
                }

                ShapeAppearanceModel shapeAppearanceModel = new ShapeAppearanceModel()
                    .ToBuilder()
                    .SetAllCorners(CornerFamily.Rounded, 4)
                    .Build();

                MaterialShapeDrawable shapeDrawable2 = new MaterialShapeDrawable(shapeAppearanceModel);

                shapeDrawable2.FillColor = Android.Content.Res.ColorStateList.ValueOf(Color.Red.ToAndroid());

                shapeDrawable2.SetStroke(4f, Color.Red.ToAndroid());

                layers[0] = shapeDrawable2;

                //var method = scrollBar.Class.GetDeclaredMethod("setVerticalThumbDrawable", Class.FromType(typeof(Drawable)));
                var method = scrollBar.Class.GetDeclaredMethod("draw", Class.FromType(typeof(Drawable)));
                method.Accessible = true;
                method.Invoke(scrollBar, layers);
            }
            catch(Exception e)
            {
            }
        }
    }
}
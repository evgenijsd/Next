using Next2.Droid.Effects;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using UIEffects = Next2.Effects;

[assembly: ExportEffect(typeof(NoOverScrollEffect), nameof(NoOverScrollEffect))]

namespace Next2.Droid.Effects
{
    public class NoOverScrollEffect : PlatformEffect
    {

        protected override void OnAttached()
        {
            if (Control != null)
            {
                var effect = (UIEffects.NoOverScrollEffect)Element.Effects
                    .FirstOrDefault(e => e is UIEffects.NoOverScrollEffect);

                if (effect.IsBouncesVisible)
                {
                    Control.OverScrollMode = Android.Views.OverScrollMode.Never;
                }
            }
        }

        protected override void OnDetached()
        {
        }
    }
}
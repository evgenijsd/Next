using System.Linq;
using Next2.Droid.Effects;
using Next2.Enums;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using CoreEffects = Next2.Effects;

[assembly: ExportEffect(typeof(NoOverScrollEffect), nameof(NoOverScrollEffect))]
namespace Next2.Droid.Effects
{
    public class NoOverScrollEffect : PlatformEffect
    {

        #region -- Overrides --

        protected override void OnAttached()
        {
            if (Control != null)
            {
                var effect = (CoreEffects.NoOverScrollEffect)Element.Effects.FirstOrDefault(e => e is CoreEffects.NoOverScrollEffect);

                if (effect != null && (effect.DisableBounceMode  == EDisableBounceMode.AndroidOnly || effect.DisableBounceMode  == EDisableBounceMode.All))
                {
                    Control.OverScrollMode = Android.Views.OverScrollMode.Never;
                }
            }
        }

        protected override void OnDetached()
        {
        }

        #endregion
    }
}
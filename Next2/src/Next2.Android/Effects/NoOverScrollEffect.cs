using Next2.Droid.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(NoOverScrollEffect), nameof(NoOverScrollEffect))]

namespace Next2.Droid.Effects
{
    public class NoOverScrollEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.OverScrollMode = Android.Views.OverScrollMode.Never;
        }

        protected override void OnDetached()
        {
        }
    }
}
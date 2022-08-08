using Next2.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(NoOverScrollEffect), nameof(NoOverScrollEffect))]
namespace Next2.iOS.Effects
{
    public class NoOverScrollEffect : PlatformEffect
    {
        #region -- Overrides --

        protected override void OnAttached()
        {
        }

        protected override void OnDetached()
        {
        }

        #endregion
    }
}
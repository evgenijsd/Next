using System.Linq;
using Next2.Enums;
using Next2.iOS.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreEffects = Next2.Effects;

[assembly: ExportEffect(typeof(NoOverScrollEffect), nameof(NoOverScrollEffect))]

namespace Next2.iOS.Effects
{
    public class NoOverScrollEffect : PlatformEffect
    {
        #region -- Overrides -- 

        protected override void OnAttached()
        {
            if (Control is UICollectionView collectionView)
            {
                var effect = (CoreEffects.NoOverScrollEffect)Element.Effects.FirstOrDefault(e => e is CoreEffects.NoOverScrollEffect);

                if (effect != null && (effect.NoOverScrollMode == EDisableBounceMode.iOSOnly || effect.NoOverScrollMode == EDisableBounceMode.Always))
                {
                    collectionView.Bounces = false;
                }
            }
        }

        protected override void OnDetached()
        {
        }

        #endregion
    }
}
using Next2.iOS.Effects;
using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIEffects = Next2.Effects;

[assembly: ResolutionGroupName("Next2.Effects")]
[assembly: ExportEffect(typeof(NoOverScrollEffect), nameof(NoOverScrollEffect))]

namespace Next2.iOS.Effects
{
    public class NoOverScrollEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
        }

        protected override void OnDetached()
        {
        }
    }
}
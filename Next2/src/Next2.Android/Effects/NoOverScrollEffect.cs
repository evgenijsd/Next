﻿using Next2.Droid.Effects;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using UIEffects = Next2.Effects;

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
                Control.OverScrollMode = Android.Views.OverScrollMode.Never;
            }
        }

        protected override void OnDetached()
        {
        }

        #endregion
    }
}
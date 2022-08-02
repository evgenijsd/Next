﻿using Next2.Enums;
using Xamarin.Forms;

namespace Next2.Effects
{
    public class NoOverScrollEffect : RoutingEffect
    {
        public NoOverScrollEffect()
            : base($"Next2.Effects.{nameof(NoOverScrollEffect)}")
        {
        }

        #region -- Public properties --

        public EDisableBounceMode NoOverScrollMode { get; set; }

        #endregion
    }
}

using Xamarin.Forms;

namespace Next2.Effects
{
    public class ScrollBarColorEffect : RoutingEffect
    {
            public ScrollBarColorEffect()
                : base($"Next2.Effects.{nameof(ScrollBarColorEffect)}")
            {
            }

            public Color ScrollBarColor { get; set; }
    }
}

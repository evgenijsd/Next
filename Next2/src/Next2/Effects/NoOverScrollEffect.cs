using Xamarin.Forms;

namespace Next2.Effects
{
    public class NoOverScrollEffect : RoutingEffect
    {
        public NoOverScrollEffect()
            : base($"Next2.Effects.{nameof(NoOverScrollEffect)}")
        {
        }

        public bool IsBouncesVisible { get; set; }
    }
}

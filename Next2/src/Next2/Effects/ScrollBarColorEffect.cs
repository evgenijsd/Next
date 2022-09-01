using Xamarin.Forms;

namespace Next2.Effects
{
    public class ScrollBarColorEffect : RoutingEffect
    {
        public ScrollBarColorEffect()
            : base("com.headworks.ScrollBarColorEffect")
        {
        }

        #region -- Public properties --

        public int ScrollBarThumbWidth { get; set; }

        public int ScrollBarCornerRadius { get; set; }

        public Color ScrollBarThumbColor { get; set; }

        public Color ScrollBarTrackColor { get; set; }

        #endregion
    }
}
using Xamarin.Forms;

namespace Next2.Controls
{
    public class CustomScrollBarCollectionView : CollectionView
    {
        public Color ScrollBarTrackColor { get; set; }

        public Color ScrollBarThumbColor { get; set; }

        public int ThumbWidth { get; set; }

        public int ScrollBarCornerRadius { get; set; }

        public int Value { get; set; }
    }
}

using Xamarin.Forms;

namespace Next2.Controls
{
    public class CustomScrollBarCollectionView : CollectionView
    {
        #region -- Public properties --

        public static readonly BindableProperty ScrollBarTrackColorProperty = BindableProperty.Create(
            propertyName: nameof(ScrollBarTrackColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomScrollBarCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public Color ScrollBarTrackColor
        {
            get => (Color)GetValue(ScrollBarTrackColorProperty);
            set => SetValue(ScrollBarTrackColorProperty, value);
        }

        public static readonly BindableProperty ScrollBarThumbColorProperty = BindableProperty.Create(
            propertyName: nameof(ScrollBarThumbColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomScrollBarCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public Color ScrollBarThumbColor
        {
            get => (Color)GetValue(ScrollBarThumbColorProperty);
            set => SetValue(ScrollBarThumbColorProperty, value);
        }

        public static readonly BindableProperty ThumbWidthProperty = BindableProperty.Create(
            propertyName: nameof(ThumbWidth),
            returnType: typeof(int),
            declaringType: typeof(CustomScrollBarCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public int ThumbWidth
        {
            get => (int)GetValue(ThumbWidthProperty);
            set => SetValue(ThumbWidthProperty, value);
        }

        public static readonly BindableProperty ScrollbarCornerRadiusProperty = BindableProperty.Create(
            propertyName: nameof(ScrollBarCornerRadius),
            returnType: typeof(int),
            declaringType: typeof(CustomScrollBarCollectionView),
            defaultBindingMode: BindingMode.TwoWay);

        public int ScrollBarCornerRadius
        {
            get => (int)GetValue(ScrollbarCornerRadiusProperty);
            set => SetValue(ScrollbarCornerRadiusProperty, value);
        }

        #endregion
    }
}

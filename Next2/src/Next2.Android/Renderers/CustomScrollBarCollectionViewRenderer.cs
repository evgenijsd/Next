using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Next2.Controls;
using Next2.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(CustomScrollBarCollectionView), typeof(CustomScrollBarCollectionViewRenderer))]
namespace Next2.Droid.Renderers
{
    public class CustomScrollBarCollectionViewRenderer : CollectionViewRenderer
    {
        protected CustomScrollBarCollectionView _customScrollBarCollectionViewElement;
        protected CustomScrollBarCollectionView CustomScrollBarCollectionViewElement => _customScrollBarCollectionViewElement ??= Element as CustomScrollBarCollectionView;

        public CustomScrollBarCollectionViewRenderer(Context context)
            : base(context)
        {
        }

        #region -- Overrides --

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
        {
            base.OnElementPropertyChanged(sender, changedProperty);

            switch (changedProperty.PropertyName)
            {
                case "Renderer":
                case nameof(_customScrollBarCollectionViewElement.ScrollBarTrackColor):
                case nameof(_customScrollBarCollectionViewElement.ScrollBarThumbColor):
                case nameof(_customScrollBarCollectionViewElement.ThumbWidth):
                    ConfigureScrollBar();
                    break;
            }
        }

        #endregion

        #region -- Protected helpers --

        protected void ConfigureScrollBar()
        {
            this.ScrollBarSize = this.CustomScrollBarCollectionViewElement.ThumbWidth;

            try
            {
                int scrollBarCornerRadius = this.CustomScrollBarCollectionViewElement.ScrollBarCornerRadius;
                Color scrollBarThumbColor = this.CustomScrollBarCollectionViewElement.ScrollBarThumbColor.ToAndroid();
                Color scrollBarTrackColor = this.CustomScrollBarCollectionViewElement.ScrollBarTrackColor.ToAndroid();

                this.VerticalScrollbarThumbDrawable = this.GetGradientDrawable(scrollBarThumbColor, scrollBarCornerRadius);
                this.VerticalScrollbarTrackDrawable = this.GetGradientDrawable(scrollBarTrackColor, scrollBarCornerRadius);
            }
            catch (Exception)
            {
            }
        }

        protected GradientDrawable GetGradientDrawable(Color color, float cornerRadius)
        {
            GradientDrawable gradient = new GradientDrawable();
            gradient.SetCornerRadius(cornerRadius);
            gradient.SetColorFilter(color, PorterDuff.Mode.SrcIn);

            return gradient;
        }

        #endregion
    }
}
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Next2.Controls;
using Next2.Droid.Renderers;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(CustomScrollBarCollectionView), typeof(CustomScrollBarCollectionViewRenderer))]
namespace Next2.Droid.Renderers
{
    public class CustomScrollBarCollectionViewRenderer : CollectionViewRenderer
    {
        protected CustomScrollBarCollectionView _customScrollBarCollectionViewElement;
        protected CustomScrollBarCollectionView CustomScrollBarCollectionViewElement => _customScrollBarCollectionViewElement ??= (Element as CustomScrollBarCollectionView);

        public CustomScrollBarCollectionViewRenderer(Context context)
            : base(context)
        {
        }

        #region -- Overrides -- 

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
        {
            base.OnElementPropertyChanged(sender, changedProperty);

            this.ScrollBarSize = this.CustomScrollBarCollectionViewElement.ThumbWidth;

            try
            {
                int scrollBarCornerRadius = this.CustomScrollBarCollectionViewElement.ScrollBarCornerRadius;
                Color scrollBarThumbColor = this.CustomScrollBarCollectionViewElement.ScrollBarThumbColor.ToAndroid();
                Color scrollBarTrackColor = this.CustomScrollBarCollectionViewElement.ScrollBarTrackColor.ToAndroid();

                this.VerticalScrollbarThumbDrawable = this.GetGradientDrawable(scrollBarThumbColor, scrollBarCornerRadius);
                this.VerticalScrollbarTrackDrawable = this.GetGradientDrawable(scrollBarTrackColor, scrollBarCornerRadius);
            }
            catch (Exception e)
            {
            }
        }

        #endregion

        #region -- Private helpers --

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
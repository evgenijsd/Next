using Next2.Controls;
using Next2.iOS.Renderers;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomFrame), typeof(CustomFrameRenderer))]
namespace Next2.iOS.Renderers
{
    public class CustomFrameRenderer : FrameRenderer
    {
        #region -- Overrides --

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var s = sender as CustomFrame;
            if (e.PropertyName == "BorderColor")
            {
                Layer.BorderColor = s.BorderColor.ToCGColor();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            if (e.NewElement is CustomFrame frame)
            {
                Layer.BorderWidth = frame.BorderWidth;
                Layer.CornerRadius = frame.CornerRadius;
                Layer.BorderColor = frame.BorderColor.ToCGColor(); 
            }

            base.OnElementChanged(e);
        }

        #endregion
    }
}
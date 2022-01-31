using Foundation;
using Next2.Controls;
using Next2.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomFrame), typeof(CustomFrameRenderer))]
namespace Next2.iOS.Renderers
{
    public class CustomFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            if (e.NewElement is CustomFrame frame)
            {
                Layer.BorderWidth = frame.BorderWidth;
                Layer.CornerRadius = frame.CornerRadius;
                Layer.BorderColor = frame.BorderColor.ToCGColor(); //Color.FromHex(hex: "#424861").ToCGColor();
            }

            base.OnElementChanged(e);
        }
    }
}
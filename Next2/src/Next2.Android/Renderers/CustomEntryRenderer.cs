using Android.Content;
using Next2.Controls;
using Next2.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace Next2.Droid.Renderers
{
    public class CustomEntryRenderer : EntryRenderer
    {
        public CustomEntryRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            Control.Background = null;
            Control.SetPadding(0, 0, 0, 0);
            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }
    }
}
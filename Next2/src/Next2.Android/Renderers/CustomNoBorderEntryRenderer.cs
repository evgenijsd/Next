using Android.Content;
using Next2.Controls;
using Next2.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomNoBorderEntry), typeof(CustomNoBorderEntryRenderer))]
namespace Next2.Droid.Renderers
{
    public class CustomNoBorderEntryRenderer : EntryRenderer
    {
        public CustomNoBorderEntryRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.Background = null;

                Control.SetPadding(0, 0, 0, 0);
            }
        }
    }
}
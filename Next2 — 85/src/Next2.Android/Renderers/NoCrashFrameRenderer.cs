using System;
using Android.Content;
using Android.Runtime;
using Next2.Droid.Renderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Frame), typeof(NoCrashFrameRenderer))]
namespace Next2.Droid.Renderers
{
    public class NoCrashFrameRenderer : Xamarin.Forms.Platform.Android.AppCompat.FrameRenderer
    {
        public NoCrashFrameRenderer(Context context)
            : base(context)
        {
        }

        [Obsolete]
        public NoCrashFrameRenderer(IntPtr handle, JniHandleOwnership transfer)
        {
        }
    }
}
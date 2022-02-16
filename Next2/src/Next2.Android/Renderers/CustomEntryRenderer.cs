using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Next2.Controls;
using Next2.Droid.Renderers;
using System;
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

            //IntPtr IntPtrtextViewClass = JNIEnv.FindClass(typeof(TextView));
            //IntPtr mCursorDrawableResProperty = JNIEnv.GetFieldID(IntPtrtextViewClass, "mCursorDrawableRes", "I");
            //JNIEnv.SetField(Control.Handle, mCursorDrawableResProperty, Resource.Drawable.my_cursor); // replace 0 with a Resource.Drawable.my_cursor

            if (Control != null)
            {
                Control.CustomSelectionActionModeCallback = new Callback();
                Control.LongClickable = false;
            }
        }
    }

    public class Callback : Java.Lang.Object, ActionMode.ICallback
    {

        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            return false;
        }

        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            return false;
        }

        public void OnDestroyActionMode(ActionMode mode)
        {

        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            return false;
        }
    }
}
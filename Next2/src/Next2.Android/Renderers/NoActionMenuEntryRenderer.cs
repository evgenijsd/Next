using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Next2.Controls;
using Next2.Droid.Renderers;
using Prism;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NoActionMenuEntry), typeof(NoActionMenuEntryRenderer))]
namespace Next2.Droid.Renderers
{
    public class NoActionMenuEntryRenderer : EntryRenderer
    {
        public NoActionMenuEntryRenderer(Context context)
            : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.CustomSelectionActionModeCallback = new Callback();
                Control.LongClickable = false;
                Control.Background = null;
                Control.SetPadding(0, 0, 0, 0);
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
            {
                Control.SetTextCursorDrawable(Resource.Drawable.custom_cursor); // This API Intrduced in android 10
            }
            else
            {
                IntPtr intPtrtextViewClass = JNIEnv.FindClass(typeof(TextView));
                IntPtr mCursorDrawableResProperty = JNIEnv.GetFieldID(intPtrtextViewClass, "mCursorDrawableRes", "I");
                JNIEnv.SetField(Control.Handle, mCursorDrawableResProperty, Resource.Drawable.custom_cursor);
            }

            if (Control != null)
            {
                Control.CustomSelectionActionModeCallback = new Callback();
                Control.CustomInsertionActionModeCallback = new Callback();
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
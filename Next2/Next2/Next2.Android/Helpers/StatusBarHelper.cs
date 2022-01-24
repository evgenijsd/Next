using Android.Views;
using AndroidX.AppCompat.App;
using Next2.Droid.Helpers;
using Next2.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(StatusBarHelper))]
namespace Next2.Droid.Helpers
{

    public class StatusBarHelper : IStatusBar
    {
        private WindowManagerFlags _originalFlags;
        private bool IsHide { get; set; }

        #region -- IStatusBar implementation --

        /// <summary>
        /// Hide
        /// </summary>
        public void HideStatusBar()
        {
            if (!IsHide)

            IsHide = true;
            {
                var activity = (AppCompatActivity)Forms.Context;
                var attrs = activity.Window.Attributes;
                _originalFlags = attrs.Flags;
                attrs.Flags |= WindowManagerFlags.Fullscreen;
                activity.Window.Attributes = attrs;
            }
        }

        /// <summary>
        /// Show
        /// </summary>
        public void ShowStatusBar()
        {
            if (IsHide)
            {
                IsHide = false;

                var activity = (AppCompatActivity)Forms.Context;
                var attrs = activity.Window.Attributes;
                attrs.Flags = _originalFlags;
                activity.Window.Attributes = attrs;
            }
        }

        #endregion
    }
}
using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using Next2.Services.Activity;
using Prism.Ioc;
using Xamarin.Forms;

namespace Next2.Droid
{
    [Activity(
        Label = "Next2",
        Icon = "@mipmap/next_icon",
        Theme = "@style/MainTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private IActivityService _activityService;

        #region -- Overrides --

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            UserDialogs.Init(this);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            Rg.Plugins.Popup.Popup.Init(this);

            RequestedOrientation = Device.Idiom == TargetIdiom.Tablet ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;

            if (Device.Idiom == TargetIdiom.Tablet)
            {
                Window.AddFlags(WindowManagerFlags.Fullscreen);
                Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
            }

            LoadApplication(new App());

            _activityService = App.Current.Container.Resolve<IActivityService>();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            if (ev.Action == MotionEventActions.Up)
            {
                _activityService.RefreshTimeLastActivity();
            }

            return base.DispatchTouchEvent(ev);
        }

        #endregion
    }
}
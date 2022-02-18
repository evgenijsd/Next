 using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using Xamarin.Forms;
using Android.Views;
using Next2.Droid.Helpers;
using System;
using System.Runtime.Remoting.Contexts;

namespace Next2.Droid
{
    [Activity(Label = "Next2", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public EventHandler GlobalTouchHandler;
        private MotionEventActions _lastMotionEventActions;
        internal static Context CurrentContext { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.AddFlags(WindowManagerFlags.Fullscreen);
            Window.ClearFlags(WindowManagerFlags.ForceNotFullscreen);
            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);

            RequestedOrientation = Device.Idiom == TargetIdiom.Tablet ? ScreenOrientation.Landscape : ScreenOrientation.Portrait;

            Xamarin.Forms.DependencyService.Register<GlobalTouchImplementation>();

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        { 
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public override bool DispatchTouchEvent(MotionEvent ev)
        {
            if (_lastMotionEventActions != MotionEventActions.Move && ev.Action == MotionEventActions.Up)
            {
                GlobalTouchHandler?.Invoke(null, null);
            }

            _lastMotionEventActions = ev.Action;

            return base.DispatchTouchEvent(ev);
        }
    }
}
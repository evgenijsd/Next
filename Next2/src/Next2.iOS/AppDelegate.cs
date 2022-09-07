using Foundation;
using Next2.Services.Activity;
using Prism.Ioc;
using UIKit;

namespace Next2.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IUIGestureRecognizerDelegate
    {
        private IActivityService _activityService;

        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        #region -- Overrides --

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Rg.Plugins.Popup.Popup.Init();

            global::Xamarin.Forms.Forms.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            LoadApplication(new App());

            _activityService = App.Current.Container.Resolve<IActivityService>();

            var isLaunchCompleted = base.FinishedLaunching(app, options);

            if (isLaunchCompleted)
            {
                UITapGestureRecognizer tap = new UITapGestureRecognizer(Self, new ObjCRuntime.Selector("gestureRecognizer:shouldReceiveTouch:"));

                tap.Delegate = (IUIGestureRecognizerDelegate)Self;

                app.KeyWindow.AddGestureRecognizer(tap);
            }

            return isLaunchCompleted;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            UIApplication.SharedApplication.StatusBarHidden = App.IsTablet;

            return App.IsTablet
                ? UIInterfaceOrientationMask.LandscapeRight
                : UIInterfaceOrientationMask.Portrait;
        }

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer gestureRecognizer, UITouch touch)
        {
            OnTapGestureAction();

            return false;
        }

        #endregion

        #region -- Private helpers --

        private void OnTapGestureAction()
        {
            _activityService.RefreshTimeLastActivity();
        }

        #endregion
    }
}
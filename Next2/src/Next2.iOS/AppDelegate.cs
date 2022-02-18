using Foundation;
using Next2.Helpers;
using UIKit;

namespace Next2.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register(nameof(AppDelegate))]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //


        #region -- Overrides --

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();

            LoadApplication(new App());

            bool ret = base.FinishedLaunching(app, options);
            if (ret)
            {
                UITapGestureRecognizer tap = new UITapGestureRecognizer(Self, new ObjCRuntime.Selector("gestureRecognizer:shouldReceiveTouch:"));
                tap.Delegate = (IUIGestureRecognizerDelegate)Self;
                app.KeyWindow.AddGestureRecognizer(tap);
            }
            return ret;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations(UIApplication application, UIWindow forWindow)
        {
            return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone
                ? UIInterfaceOrientationMask.Portrait
                : UIInterfaceOrientationMask.LandscapeRight;
        }

        #endregion

        [Export("gestureRecognizer:shouldReceiveTouch:")]
        public bool ShouldReceiveTouch(UIGestureRecognizer gestureRecognizer, UITouch touch)
        {
            Xamarin.Forms.DependencyService.Get<IGlobalTouch>().TapScreen();
            return false;
        }
    }
}

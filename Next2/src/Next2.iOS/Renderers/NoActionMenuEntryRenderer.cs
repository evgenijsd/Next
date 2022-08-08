using Foundation;
using Next2.Controls;
using Next2.iOS.Renderers;
using ObjCRuntime;
using System.Drawing;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NoActionMenuEntry), typeof(NoActionMenuEntryRenderer))]
namespace Next2.iOS.Renderers
{
    public class NoActionMenuEntryRenderer : EntryRenderer
    {
        #region -- Overrides --

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.BorderStyle = UIKit.UITextBorderStyle.None;
                Control.TintColor = UIColor.SystemRedColor;
            }

            if (Element == null)
            {
                return;
            }

            if (Element.Keyboard == Keyboard.Numeric)
            {
                AddDoneButton();
            }
        }

        public override bool CanPerform(Selector action, NSObject withSender)
        {
            NSOperationQueue.MainQueue.AddOperation(() => { UIMenuController.SharedMenuController.SetMenuVisible(false, false); });

            return base.CanPerform(action, withSender);
        }

        #endregion

        #region -- Private helpers --

        private void AddDoneButton()
        {
            var toolbar = new UIToolbar(new RectangleF(0.0f, 0.0f, 50.0f, 44.0f));

            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (sender, e) =>
            {
                Control.ResignFirstResponder();
                var baseEntry = Element.GetType();
                ((IEntryController)Element).SendCompleted();
            });

            toolbar.Items = new UIBarButtonItem[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                doneButton,
            };

            Control.InputAccessoryView = toolbar;
        }

        #endregion
    }
}
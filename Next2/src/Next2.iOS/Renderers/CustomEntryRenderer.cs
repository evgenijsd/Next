using System;
using System.Drawing;
using Foundation;
using Next2.Controls;
using Next2.iOS.Renderers;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace Next2.iOS.Renderers
{
    class CustomEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                Control.BorderStyle = UIKit.UITextBorderStyle.None;
            }

            if (this.Element == null)
                return;

            // Check only for Numeric keyboard
            if (this.Element.Keyboard == Keyboard.Numeric)
                this.AddDoneButton();
        }

        #region -- Overrides --

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
                this.Control.ResignFirstResponder();
                var baseEntry = this.Element.GetType();
                ((IEntryController)Element).SendCompleted();
            });

            toolbar.Items = new UIBarButtonItem[] {
                new UIBarButtonItem (UIBarButtonSystemItem.FlexibleSpace),
                doneButton
            };
            this.Control.InputAccessoryView = toolbar;
        }

        #endregion

    }
} 
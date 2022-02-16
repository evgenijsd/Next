using System;
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
    public class CustomEntryRenderer : EntryRenderer
    {
        /// <inheritdoc/>
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                this.Control.BorderStyle = UIKit.UITextBorderStyle.None;
                this.Control.TintColor = UIColor.SystemRedColor;
            }
        }

        /// <inheritdoc/>
        public override bool CanPerform(Selector action, NSObject withSender)
        {
            NSOperationQueue.MainQueue.AddOperation(() => { UIMenuController.SharedMenuController.SetMenuVisible(false, false); });

            return base.CanPerform(action, withSender);
        }
    }
} 
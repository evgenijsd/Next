using Android.Content;
using Next2.Controls;
using Next2.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LineSpacingLabel), typeof(LineSpacingLabelRenderer))]
namespace Next2.Droid.Renderers
{
    public class LineSpacingLabelRenderer : LabelRenderer
    {
        protected LineSpacingLabel _lineSpacingLabel;
        protected LineSpacingLabel LineSpacingLabel => _lineSpacingLabel ??= (Element as LineSpacingLabel);

        public LineSpacingLabelRenderer(Context context)
            : base(context)
        {
        }

        #region -- Overrides --

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            ConfigureLineSpacing();
        }

        #endregion

        #region -- Private hepers

        protected void ConfigureLineSpacing()
        {
            if (LineSpacingLabel != null)
            {
                var lineSpacing = LineSpacingLabel.LineSpacing;

                Control.SetLineSpacing(1f, lineSpacing);

                UpdateLayout();
            }
        }

        #endregion
    }
}
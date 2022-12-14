using Xamarin.Forms;

namespace Next2.Controls.StateContainer.Animation
{
    public class FadeOutAnimation : AnimationBase
    {
        #region -- Overrides --

        public override void Apply(View view)
        {
            if (view != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    view.FadeTo(0, 1);
                });
            }
        }

        #endregion
    }
}
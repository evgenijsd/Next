using Xamarin.Forms;

namespace Next2.Controls.StateContainer.Animation
{
    public class FadeInAnimation : AnimationBase
    {
        #region -- Overrides --

        public override void Apply(View view)
        {
            if (view != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    view.FadeTo(1);
                });
            }
        }

        #endregion
    }
}
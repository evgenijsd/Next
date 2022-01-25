using Xamarin.Forms;

namespace MapNotePad.Controls.StateContainer.Animation
{
    public class FadeInAnimation : AnimationBase
    {
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
    }
}
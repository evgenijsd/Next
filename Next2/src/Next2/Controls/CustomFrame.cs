using Xamarin.Forms;

namespace Next2.Controls
{
    public class CustomFrame : Frame
    {
        #region -- Public properties --

        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(
            propertyName: nameof(BorderWidth),
            returnType: typeof(float),
            declaringType: typeof(CustomFrame),
            defaultBindingMode: BindingMode.OneWay);

        public float BorderWidth
        {
            get => (float)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }

        #endregion
    }
}

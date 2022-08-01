using Xamarin.Forms;

namespace Next2.Controls
{
    public class LineSpacingLabel : Label
    {
        #region -- Public properties --

        public static readonly BindableProperty LineSpacingProperty = BindableProperty.Create(
            propertyName: nameof(LineSpacing),
            returnType: typeof(float),
            declaringType: typeof(LineSpacingLabel),
            defaultBindingMode: BindingMode.OneWay);

        public float LineSpacing
        {
            get => (float)GetValue(LineSpacingProperty);
            set => SetValue(LineSpacingProperty, value);
        }

        #endregion
    }
}

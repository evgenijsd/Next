using Xamarin.Forms;

namespace Next2.Controls
{
    public class NoActionMenuEntry : Entry
    {
        #region -- Public properties --

        public static readonly BindableProperty IsValidProperty = BindableProperty.Create(
           propertyName: nameof(IsValidProperty),
           returnType: typeof(bool),
           declaringType: typeof(NoActionMenuEntry),
           defaultBindingMode: BindingMode.TwoWay);

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }

        #endregion
    }
}

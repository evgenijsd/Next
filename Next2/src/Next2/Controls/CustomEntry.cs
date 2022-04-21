using Xamarin.Forms;

namespace Next2.Controls
{
    public class CustomEntry : Entry
    {
        public static readonly BindableProperty IsValidProperty = BindableProperty.Create(
           propertyName: nameof(IsValidProperty),
           returnType: typeof(bool),
           declaringType: typeof(CustomEntry),
           defaultBindingMode: BindingMode.TwoWay);

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }
    }
}

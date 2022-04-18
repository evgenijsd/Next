using Xamarin.Forms;

namespace Next2.Controls
{
    public class HideClipboardEntry : Entry
    {
        public static readonly BindableProperty IsValidProperty = BindableProperty.Create(
           propertyName: nameof(IsValidProperty),
           returnType: typeof(bool),
           declaringType: typeof(HideClipboardEntry),
           defaultBindingMode: BindingMode.TwoWay);

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }
    }
}

using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class CustomButtonOrderRight : Grid
    {
        public CustomButtonOrderRight()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            propertyName: nameof(CornerRadius),
            returnType: typeof(float),
            declaringType: typeof(CustomButtonOrderRight),
            defaultValue: 0F,
            defaultBindingMode: BindingMode.TwoWay);

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
    }
}
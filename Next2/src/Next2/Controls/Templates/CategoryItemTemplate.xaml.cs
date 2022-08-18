using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class CategoryItemTemplate : StackLayout
    {
        public CategoryItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemColorProperty = BindableProperty.Create(
            propertyName: nameof(ItemColor),
            returnType: typeof(Color),
            declaringType: typeof(CategoryItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public Color ItemColor
        {
            get => (Color)GetValue(ItemColorProperty);
            set => SetValue(ItemColorProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(CategoryItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(CategoryItemTemplate),
            defaultValue: 12d,
            defaultBindingMode: BindingMode.OneWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            declaringType: typeof(CategoryItemTemplate),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        #endregion
    }
}
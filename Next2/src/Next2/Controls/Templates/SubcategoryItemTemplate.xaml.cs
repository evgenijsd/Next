using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class SubCategoryItemTemplate : StackLayout
    {
        public SubCategoryItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(SubCategoryItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(float),
            declaringType: typeof(SubCategoryItemTemplate),
            defaultValue: 12f,
            defaultBindingMode: BindingMode.OneWay);

        public float FontSize
        {
            get => (float)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            declaringType: typeof(SubCategoryItemTemplate),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly BindableProperty TextMarginProperty = BindableProperty.Create(
            propertyName: nameof(TextMargin),
            returnType: typeof(Thickness),
            defaultValue: new Thickness(0),
            declaringType: typeof(SubCategoryItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public Thickness TextMargin
        {
            get => (Thickness)GetValue(TextMarginProperty);
            set => SetValue(TextMarginProperty, value);
        }

        #endregion
    }
}
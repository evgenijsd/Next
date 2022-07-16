using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace Next2.Controls.Buttons
{
    public partial class IconTextButton : PancakeView
    {
        public IconTextButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
            propertyName: nameof(IconSource),
            returnType: typeof(string),
            declaringType: typeof(IconTextButton),
            defaultBindingMode: BindingMode.TwoWay);

        public string IconSource
        {
            get => (string)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
            propertyName: nameof(IconSize),
            returnType: typeof(Size),
            defaultValue: new Size(18, 18),
            declaringType: typeof(IconTextButton));

        public Size IconSize
        {
            get => (Size)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
            propertyName: nameof(Spacing),
            returnType: typeof(double),
            defaultValue: 12d,
            declaringType: typeof(IconTextButton),
            defaultBindingMode: BindingMode.OneWay);

        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            defaultValue: 14d,
            declaringType: typeof(IconTextButton),
            defaultBindingMode: BindingMode.OneWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            defaultValue: "Barlow-Medium",
            declaringType: typeof(IconTextButton),
            defaultBindingMode: BindingMode.OneWay);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(IconTextButton),
            defaultBindingMode: BindingMode.OneWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion
    }
}
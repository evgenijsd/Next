using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class CustomSelectedButton : Frame
    {
        public CustomSelectedButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            propertyName: nameof(IsSelected),
            returnType: typeof(bool),
            declaringType: typeof(CustomSelectedButton));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(CustomSelectedButton));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomSelectedButton),
            defaultValue: Color.White,
            defaultBindingMode: BindingMode.TwoWay);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(float),
            declaringType: typeof(CustomSelectedButton),
            defaultValue: 12f,
            defaultBindingMode: BindingMode.TwoWay);

        public float FontSize
        {
            get => (float)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            declaringType: typeof(CustomSelectedButton),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly BindableProperty SelectedBackgroundColorProperty = BindableProperty.Create(
            propertyName: nameof(SelectedBackgroundColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomSelectedButton),
            defaultValue: Color.Orange,
            defaultBindingMode: BindingMode.TwoWay);

        public Color SelectedBackgroundColor
        {
            get => (Color)GetValue(SelectedBackgroundColorProperty);
            set => SetValue(SelectedBackgroundColorProperty, value);
        }

        public static readonly BindableProperty SelectedBorderColorProperty = BindableProperty.Create(
            propertyName: nameof(SelectedBorderColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomSelectedButton),
            defaultValue: Color.Orange,
            defaultBindingMode: BindingMode.TwoWay);

        public Color SelectedBorderColor
        {
            get => (Color)GetValue(SelectedBorderColorProperty);
            set => SetValue(SelectedBorderColorProperty, value);
        }

        #endregion
    }
}
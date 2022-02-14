using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class CustomButton : Grid
    {
        public CustomButton()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            propertyName: nameof(CornerRadius),
            returnType: typeof(float),
            declaringType: typeof(CustomButton),
            defaultValue: 0F,
            defaultBindingMode: BindingMode.TwoWay);

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(CustomButton),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
            propertyName: nameof(ImageSource),
            returnType: typeof(string),
            declaringType: typeof(CustomButton),
            defaultValue: "ic_search_24x24",
            defaultBindingMode: BindingMode.TwoWay);

        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
            propertyName: nameof(BorderColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomButton),
            defaultBindingMode: BindingMode.TwoWay);

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public static readonly BindableProperty BackColorProperty = BindableProperty.Create(
            propertyName: nameof(BackColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomButton),
            defaultBindingMode: BindingMode.TwoWay);

        public Color BackColor
        {
            get => (Color)GetValue(BackColorProperty);
            set => SetValue(BackColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomButton),
            defaultBindingMode: BindingMode.TwoWay);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty OpacityTextProperty = BindableProperty.Create(
            propertyName: nameof(OpacityText),
            returnType: typeof(double),
            declaringType: typeof(CustomButton),
            defaultBindingMode: BindingMode.TwoWay);

        public double OpacityText
        {
            get => (double)GetValue(OpacityTextProperty);
            set => SetValue(OpacityProperty, value);
        }

        public static readonly BindableProperty IsVisibleSearchProperty = BindableProperty.Create(
            propertyName: nameof(IsVisibleSearch),
            returnType: typeof(bool),
            declaringType: typeof(CustomButton),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsVisibleSearch
        {
            get => (bool)GetValue(IsVisibleSearchProperty);
            set => SetValue(IsVisibleSearchProperty, value);
        }

        public static readonly BindableProperty CommandSearchProperty = BindableProperty.Create(
            propertyName: nameof(CommandSearch),
            returnType: typeof(ICommand),
            declaringType: typeof(CustomButton),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand CommandSearch
        {
            get => (ICommand)GetValue(CommandSearchProperty);
            set => SetValue(CommandSearchProperty, value);
        }

        public static readonly BindableProperty CommandClearProperty = BindableProperty.Create(
            propertyName: nameof(CommandClear),
            returnType: typeof(ICommand),
            declaringType: typeof(CustomButton),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand CommandClear
        {
            get => (ICommand)GetValue(CommandClearProperty);
            set => SetValue(CommandClearProperty, value);
        }
    }
}
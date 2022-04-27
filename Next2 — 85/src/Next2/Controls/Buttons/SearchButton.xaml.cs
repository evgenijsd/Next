using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class SearchButton : Grid
    {
        public SearchButton()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            propertyName: nameof(CornerRadius),
            returnType: typeof(float),
            declaringType: typeof(SearchButton),
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
            declaringType: typeof(SearchButton),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
            propertyName: nameof(Placeholder),
            returnType: typeof(string),
            declaringType: typeof(SearchButton),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay);

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(
            propertyName: nameof(ImageSource),
            returnType: typeof(string),
            declaringType: typeof(SearchButton),
            defaultValue: "ic_close_square_24x24",
            defaultBindingMode: BindingMode.TwoWay);

        public string ImageSource
        {
            get => (string)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
            propertyName: nameof(BorderColor),
            returnType: typeof(Color),
            declaringType: typeof(SearchButton),
            defaultBindingMode: BindingMode.TwoWay);

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public static readonly BindableProperty BackColorProperty = BindableProperty.Create(
            propertyName: nameof(BackColor),
            returnType: typeof(Color),
            declaringType: typeof(SearchButton),
            defaultBindingMode: BindingMode.TwoWay);

        public Color BackColor
        {
            get => (Color)GetValue(BackColorProperty);
            set => SetValue(BackColorProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(SearchButton),
            defaultBindingMode: BindingMode.TwoWay);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty CommandSearchProperty = BindableProperty.Create(
            propertyName: nameof(CommandSearch),
            returnType: typeof(ICommand),
            declaringType: typeof(SearchButton),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand CommandSearch
        {
            get => (ICommand)GetValue(CommandSearchProperty);
            set => SetValue(CommandSearchProperty, value);
        }

        public static readonly BindableProperty CommandClearProperty = BindableProperty.Create(
            propertyName: nameof(CommandClear),
            returnType: typeof(ICommand),
            declaringType: typeof(SearchButton),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand CommandClear
        {
            get => (ICommand)GetValue(CommandClearProperty);
            set => SetValue(CommandClearProperty, value);
        }
    }
}
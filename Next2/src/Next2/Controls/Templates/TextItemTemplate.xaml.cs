using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class TextItemTemplate : Grid
    {
        public TextItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(TextItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty FormattedTextProperty = BindableProperty.Create(
            propertyName: nameof(FormattedText),
            returnType: typeof(FormattedString),
            declaringType: typeof(TextItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public FormattedString FormattedText
        {
            get => (FormattedString)GetValue(FormattedTextProperty);
            set => SetValue(FormattedTextProperty, value);
        }

        public static readonly BindableProperty TextMarginProperty = BindableProperty.Create(
            propertyName: nameof(TextMargin),
            returnType: typeof(Thickness),
            defaultValue: new Thickness(14d, 0d),
            declaringType: typeof(TextItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public Thickness TextMargin
        {
            get => (Thickness)GetValue(TextMarginProperty);
            set => SetValue(TextMarginProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(TextItemTemplate),
            defaultValue: 14d,
            defaultBindingMode: BindingMode.OneWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        #endregion
    }
}
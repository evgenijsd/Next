using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class CustomNavigationbar : StackLayout
    {
        public CustomNavigationbar()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty HeightImageProperty = BindableProperty.Create(
            propertyName: nameof(HeightImage),
            returnType: typeof(double),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public double HeightImage
        {
            get => (double)GetValue(HeightImageProperty);
            set => SetValue(HeightImageProperty, value);
        }

        public static readonly BindableProperty LeftButtonCommandProperty = BindableProperty.Create(
            propertyName: nameof(LeftButtonCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand LeftButtonCommand
        {
            get => (ICommand)GetValue(LeftButtonCommandProperty);
            set => SetValue(LeftButtonCommandProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty TitleColorProperty = BindableProperty.Create(
            propertyName: nameof(TitleColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public Color TitleColor
        {
            get => (Color)GetValue(TitleColorProperty);
            set => SetValue(TitleColorProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            declaringType: typeof(CustomNavigationbar),
            defaultValue: "Barlow-SemiBold",
            defaultBindingMode: BindingMode.TwoWay);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        #endregion
    }
}
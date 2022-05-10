using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Buttons
{
    public partial class InputButton : CustomFrame
    {
        public InputButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            declaringType: typeof(InputButton),
            defaultValue: string.Empty);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(InputButton),
            defaultValue: 12d);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty LeftImagePathProperty = BindableProperty.Create(
            propertyName: nameof(LeftImagePath),
            returnType: typeof(string),
            declaringType: typeof(InputButton));

        public string LeftImagePath
        {
            get => (string)GetValue(LeftImagePathProperty);
            set => SetValue(LeftImagePathProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(InputButton));

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(InputButton));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty IsLeftImageVisibleProperty = BindableProperty.Create(
            propertyName: nameof(IsLeftImageVisible),
            returnType: typeof(bool),
            declaringType: typeof(InputButton));

        public bool IsLeftImageVisible
        {
            get => (bool)GetValue(IsLeftImageVisibleProperty);
            set => SetValue(IsLeftImageVisibleProperty, value);
        }

        public static readonly BindableProperty TapGestureRecognizerCommandProperty = BindableProperty.Create(
            propertyName: nameof(TapGestureRecognizerCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(InputButton));

        public ICommand TapGestureRecognizerCommand
        {
            get => (ICommand)GetValue(TapGestureRecognizerCommandProperty);
            set => SetValue(TapGestureRecognizerCommandProperty, value);
        }

        public static readonly BindableProperty IsValidValueProperty = BindableProperty.Create(
            propertyName: nameof(IsValidValue),
            returnType: typeof(bool),
            defaultValue: true,
            declaringType: typeof(InputButton));

        public bool IsValidValue
        {
            get => (bool)GetValue(IsValidValueProperty);
            set => SetValue(IsValidValueProperty, value);
        }

        #endregion
    }
}
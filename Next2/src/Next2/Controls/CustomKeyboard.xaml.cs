using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class CustomKeyboard : ContentView
    {
        public CustomKeyboard()
        {
            InitializeComponent();
        }

        #region -- Public Properties --

        public static readonly BindableProperty ButtonBackgroundColorProperty = BindableProperty.Create(
             propertyName: nameof(ButtonBackgroundColor),
             returnType: typeof(Color),
             declaringType: typeof(NumericKeyboard),
             defaultBindingMode: BindingMode.TwoWay);

        public Color ButtonBackgroundColor
        {
            get => (Color)GetValue(ButtonBackgroundColorProperty);
            set => SetValue(ButtonBackgroundColorProperty, value);
        }

        public static readonly BindableProperty PressNumberButtonCommandProperty = BindableProperty.Create(
          propertyName: nameof(PressNumberButtonCommand),
          returnType: typeof(ICommand),
          declaringType: typeof(CustomKeyboard),
          defaultBindingMode: BindingMode.TwoWay);

        public ICommand PressNumberButtonCommand
        {
            get => (ICommand)GetValue(PressNumberButtonCommandProperty);
            set => SetValue(PressNumberButtonCommandProperty, value);
        }

        public static readonly BindableProperty PressClearButtonCommandProperty = BindableProperty.Create(
          propertyName: nameof(PressClearButtonCommand),
          returnType: typeof(ICommand),
          declaringType: typeof(CustomKeyboard),
          defaultBindingMode: BindingMode.TwoWay);

        public ICommand PressClearButtonCommand
        {
            get => (ICommand)GetValue(PressClearButtonCommandProperty);
            set => SetValue(PressClearButtonCommandProperty, value);
        }

        #endregion
    }
}
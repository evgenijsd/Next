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

        public static readonly BindableProperty ButtonTapCommandProperty = BindableProperty.Create(
          propertyName: nameof(ButtonTapCommand),
          returnType: typeof(ICommand),
          declaringType: typeof(CustomKeyboard),
          defaultBindingMode: BindingMode.TwoWay);

        public ICommand ButtonTapCommand
        {
            get => (ICommand)GetValue(ButtonTapCommandProperty);
            set => SetValue(ButtonTapCommandProperty, value);
        }

        public static readonly BindableProperty ButtonClearTapCommandProperty = BindableProperty.Create(
          propertyName: nameof(ButtonClearTapCommand),
          returnType: typeof(ICommand),
          declaringType: typeof(CustomKeyboard),
          defaultBindingMode: BindingMode.TwoWay);

        public ICommand ButtonClearTapCommand
        {
            get => (ICommand)GetValue(ButtonClearTapCommandProperty);
            set => SetValue(ButtonClearTapCommandProperty, value);
        }

        #endregion
    }
}
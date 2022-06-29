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

        #region -- Public Properties

        public static readonly BindableProperty BackgroundColorButtonProperty = BindableProperty.Create(
           propertyName: nameof(BackgroundColorButton),
           returnType: typeof(Color),
           declaringType: typeof(CustomKeyboard),
           defaultBindingMode: BindingMode.TwoWay);

        public Color BackgroundColorButton
        {
            get => (Color)GetValue(BackgroundColorButtonProperty);
            set => SetValue(BackgroundColorButtonProperty, value);
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
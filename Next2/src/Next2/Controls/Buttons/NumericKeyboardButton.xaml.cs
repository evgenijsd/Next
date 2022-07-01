using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Buttons
{
    public partial class NumericKeyboardButton : CustomFrame
    {
        public NumericKeyboardButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
         propertyName: nameof(Title),
         returnType: typeof(string),
         declaringType: typeof(NumericKeyboardButton),
         defaultBindingMode: BindingMode.TwoWay);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty TapGestureRecognizerCommandProperty = BindableProperty.Create(
            propertyName: nameof(TapGestureRecognizerCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(NumericKeyboardButton),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand TapGestureRecognizerCommand
        {
            get => (ICommand)GetValue(TapGestureRecognizerCommandProperty);
            set => SetValue(TapGestureRecognizerCommandProperty, value);
        }

        #endregion
    }
}
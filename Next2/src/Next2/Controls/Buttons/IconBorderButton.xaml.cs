using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Buttons
{
    public partial class IconBorderButton : Frame
    {
        public IconBorderButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
            propertyName: nameof(IconSource),
            returnType: typeof(string),
            declaringType: typeof(IconBorderButton),
            defaultBindingMode: BindingMode.TwoWay);

        public string IconSource
        {
            get => (string)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public static readonly BindableProperty IconSizesProperty = BindableProperty.Create(
            propertyName: nameof(IconSizes),
            returnType: typeof(int),
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public int IconSizes
        {
            get => (int)GetValue(IconSizesProperty);
            set => SetValue(IconSizesProperty, value);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            propertyName: nameof(Command),
            returnType: typeof(ICommand),
            declaringType: typeof(IconBorderButton),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        #endregion
    }
}
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

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

        public static readonly BindableProperty IconWidthProperty = BindableProperty.Create(
            propertyName: nameof(IconWidth),
            returnType: typeof(int),
            declaringType: typeof(IconBorderButton),
            defaultBindingMode: BindingMode.TwoWay);

        public int IconWidth
        {
            get => (int)GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }

        public static readonly BindableProperty IconHeightProperty = BindableProperty.Create(
            propertyName: nameof(IconHeight),
            returnType: typeof(int),
            declaringType: typeof(IconBorderButton),
            defaultBindingMode: BindingMode.TwoWay);

        public int IconHeight
        {
            get => (int)GetValue(IconHeightProperty);
            set => SetValue(IconHeightProperty, value);
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
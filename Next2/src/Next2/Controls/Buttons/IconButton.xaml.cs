using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace Next2.Controls.Buttons
{
    public partial class IconButton : PancakeView
    {
        public IconButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
            propertyName: nameof(IconSource),
            returnType: typeof(string),
            declaringType: typeof(IconButton),
            defaultBindingMode: BindingMode.OneWay);

        public string IconSource
        {
            get => (string)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
            propertyName: nameof(IconSize),
            returnType: typeof(Size),
            defaultValue: new Size(28, 28),
            declaringType: typeof(IconButton),
            defaultBindingMode: BindingMode.OneWay);

        public Size IconSize
        {
            get => (Size)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        #endregion
    }
}
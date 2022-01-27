using Xamarin.Forms;

namespace Next2.Controls.Buttons
{
    public partial class CustomBorderButton : Frame
    {
        public CustomBorderButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(CustomBorderButton));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        #endregion
    }
}
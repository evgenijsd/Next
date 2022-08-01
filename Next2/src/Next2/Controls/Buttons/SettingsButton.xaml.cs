using Xamarin.Forms;

namespace Next2.Controls.Buttons
{
    public partial class SettingsButton : Frame
    {
        public SettingsButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(SettingsButton),
            defaultBindingMode: BindingMode.OneWay);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion
    }
}
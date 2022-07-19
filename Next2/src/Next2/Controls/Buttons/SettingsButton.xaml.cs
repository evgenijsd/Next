using System.Windows.Input;
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
         defaultBindingMode: BindingMode.TwoWay);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(
            propertyName: nameof(TapCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(SettingsButton),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand TapCommand
        {
            get => (ICommand)GetValue(TapCommandProperty);
            set => SetValue(TapCommandProperty, value);
        }

        public static readonly BindableProperty TapCommandParameterProperty = BindableProperty.Create(
            propertyName: nameof(TapCommandParameter),
            returnType: typeof(object),
            declaringType: typeof(SettingsButton),
            defaultBindingMode: BindingMode.TwoWay);

        public object TapCommandParameter
        {
            get => GetValue(TapCommandParameterProperty);
            set => SetValue(TapCommandParameterProperty, value);
        }

        #endregion

    }
}
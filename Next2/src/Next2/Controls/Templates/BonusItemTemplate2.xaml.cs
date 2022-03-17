using Next2.Models;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class BonusItemTemplate2 : StackLayout
    {
        public BonusItemTemplate2()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(BonusItemTemplate2));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TapSelectCommandProperty = BindableProperty.Create(
            propertyName: nameof(TapSelectCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(BonusItemTemplate2),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand TapSelectCommand
        {
            get => (ICommand)GetValue(TapSelectCommandProperty);
            set => SetValue(TapSelectCommandProperty, value);
        }

        public static readonly BindableProperty TapSelectCommandParameterProperty = BindableProperty.Create(
            propertyName: nameof(TapSelectCommandParameter),
            returnType: typeof(BonusBindableModel),
            declaringType: typeof(BonusItemTemplate2),
            defaultBindingMode: BindingMode.TwoWay);

        public object TapSelectCommandParameter
        {
            get => (BonusBindableModel)GetValue(TapSelectCommandParameterProperty);
            set => SetValue(TapSelectCommandParameterProperty, value);
        }

        #endregion
    }
}
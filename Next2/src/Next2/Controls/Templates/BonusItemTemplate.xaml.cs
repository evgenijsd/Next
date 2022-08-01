using Next2.Models;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class BonusItemTemplate : StackLayout
    {
        public BonusItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty HeightBonusProperty = BindableProperty.Create(
            propertyName: nameof(HeightBonus),
            returnType: typeof(double),
            defaultValue: 50.0d,
            declaringType: typeof(BonusItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public double HeightBonus
        {
            get => (double)GetValue(HeightBonusProperty);
            set => SetValue(HeightBonusProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(BonusItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TapSelectCommandProperty = BindableProperty.Create(
            propertyName: nameof(TapSelectCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(BonusItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public ICommand TapSelectCommand
        {
            get => (ICommand)GetValue(TapSelectCommandProperty);
            set => SetValue(TapSelectCommandProperty, value);
        }

        public static readonly BindableProperty TapSelectCommandParameterProperty = BindableProperty.Create(
            propertyName: nameof(TapSelectCommandParameter),
            returnType: typeof(BonusBindableModel),
            declaringType: typeof(BonusItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public object TapSelectCommandParameter
        {
            get => (BonusBindableModel)GetValue(TapSelectCommandParameterProperty);
            set => SetValue(TapSelectCommandParameterProperty, value);
        }

        public static readonly BindableProperty ImagePathProperty = BindableProperty.Create(
            propertyName: nameof(ImagePath),
            returnType: typeof(string),
            defaultValue: "ic_check_box_unchecked_white",
            declaringType: typeof(BonusItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public static readonly BindableProperty BorderBonusColorProperty = BindableProperty.Create(
            propertyName: nameof(BorderBonusColor),
            returnType: typeof(Color),
            declaringType: typeof(BonusItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public Color BorderBonusColor
        {
            get => (Color)GetValue(BorderBonusColorProperty);
            set => SetValue(BorderBonusColorProperty, value);
        }

        public static readonly BindableProperty BackColorProperty = BindableProperty.Create(
            propertyName: nameof(BackColor),
            returnType: typeof(Color),
            declaringType: typeof(BonusItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public Color BackColor
        {
            get => (Color)GetValue(BackColorProperty);
            set => SetValue(BackColorProperty, value);
        }

        #endregion
    }
}
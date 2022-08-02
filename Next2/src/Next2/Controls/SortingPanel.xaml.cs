using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class SortingPanel : StackLayout
    {
        public SortingPanel()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(SortingPanel),
            defaultBindingMode: BindingMode.OneWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(SortingPanel),
            defaultValue: 12d,
            defaultBindingMode: BindingMode.OneWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            declaringType: typeof(SortingPanel),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWay);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly BindableProperty ChangingOrderSortCommandProperty = BindableProperty.Create(
            propertyName: nameof(ChangingOrderSortCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(SortingPanel),
            defaultValue: default(ICommand),
            defaultBindingMode: BindingMode.OneWay);

        public ICommand ChangingOrderSortCommand
        {
            get => (ICommand)GetValue(ChangingOrderSortCommandProperty);
            set => SetValue(ChangingOrderSortCommandProperty, value);
        }

        #endregion
    }
}
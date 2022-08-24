using Next2.Helpers;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class HoldTimeItemTemplate : Grid
    {
        public HoldTimeItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemBackgroundColorProperty = BindableProperty.Create(
            propertyName: nameof(ItemBackgroundColor),
            returnType: typeof(Color),
            declaringType: typeof(HoldTimeItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public Color ItemBackgroundColor
        {
            get => (Color)GetValue(ItemBackgroundColorProperty);
            set => SetValue(ItemBackgroundColorProperty, value);
        }

        public static readonly BindableProperty ItemBorderColorProperty = BindableProperty.Create(
            propertyName: nameof(ItemBorderColor),
            returnType: typeof(Color),
            declaringType: typeof(HoldTimeItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public Color ItemBorderColor
        {
            get => (Color)GetValue(ItemBorderColorProperty);
            set => SetValue(ItemBorderColorProperty, value);
        }

        #endregion
    }
}
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

        public static readonly BindableProperty HeightItemsProperty = BindableProperty.Create(
            propertyName: nameof(ItemHeight),
            returnType: typeof(double),
            defaultValue: 40d,
            declaringType: typeof(HoldTimeItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public double ItemHeight
        {
            get => (double)GetValue(HeightItemsProperty);
            set => SetValue(HeightItemsProperty, value);
        }

        #endregion
    }
}
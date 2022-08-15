using Next2.Helpers;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class TimeItemTemplate : Grid
    {
        public TimeItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty HeightItemsProperty = BindableProperty.Create(
            propertyName: nameof(HeightItems),
            returnType: typeof(double),
            defaultValue: 70d,
            declaringType: typeof(TimeItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public double HeightItems
        {
            get => (double)GetValue(HeightItemsProperty);
            set => SetValue(HeightItemsProperty, value);
        }

        #endregion
    }
}
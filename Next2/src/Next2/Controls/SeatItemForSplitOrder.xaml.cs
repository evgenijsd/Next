using System.Collections;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class SeatItemForSplitOrder : StackLayout
    {
        public SeatItemForSplitOrder()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(ICollection),
            typeof(SeatItemForSplitOrder),
            default(ICollection));

        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty SubItemColorProperty = BindableProperty.Create(
            propertyName: nameof(SubItemColor),
            returnType: typeof(Color),
            declaringType: typeof(SeatItemForSplitOrder));

        public Color SubItemColor
        {
            get => (Color)GetValue(SubItemColorProperty);
            set => SetValue(SubItemColorProperty, value);
        }

        #endregion
    }
}
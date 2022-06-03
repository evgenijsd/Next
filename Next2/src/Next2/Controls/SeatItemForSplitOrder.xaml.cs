using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
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
            default(ICollection),
            propertyChanged: (b, o, n) =>
            ((SeatItemForSplitOrder)b).OnItemsSourcePropertyChanged((ICollection)o, (ICollection)n));

        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty HeightListProperty = BindableProperty.Create(
            propertyName: nameof(HeightList),
            returnType: typeof(double),
            declaringType: typeof(SeatItemForSplitOrder));

        public double HeightList
        {
            get => (double)GetValue(HeightListProperty);
            set => SetValue(HeightListProperty, value);
        }

        #endregion

        #region -- Private methods --

        private void OnItemsSourcePropertyChanged(ICollection oldItemsSource, ICollection newItemsSource)
        {
            if (oldItemsSource is INotifyCollectionChanged ncc)
            {
                ncc.CollectionChanged -= OnItemsSourceCollectionChanged;
            }

            if (newItemsSource is INotifyCollectionChanged ncc1)
            {
                ncc1.CollectionChanged += OnItemsSourceCollectionChanged;
            }

            if (ItemsSource is not null)
            {
                HeightList = ItemsSource.Count * 95;
            }
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemsSource is not null)
            {
                HeightList = ItemsSource.Count * 95;
            }
        }

        #endregion
    }
}
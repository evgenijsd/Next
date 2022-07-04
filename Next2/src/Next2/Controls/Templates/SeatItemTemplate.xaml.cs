using Next2.Models;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class SeatItemTemplate : StackLayout
    {
        public SeatItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(ICollection),
            typeof(SeatItemTemplate),
            default(ICollection),
            propertyChanged: (b, o, n) =>
            ((SeatItemTemplate)b).OnItemsSourcePropertyChanged((ICollection)o, (ICollection)n));

        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static readonly BindableProperty HeightListProperty = BindableProperty.Create(
            propertyName: nameof(HeightList),
            returnType: typeof(double),
            declaringType: typeof(SeatItemTemplate));

        public double HeightList
        {
            get => (double)GetValue(HeightListProperty);
            set => SetValue(HeightListProperty, value);
        }

        #endregion

        #region -- Private helpers --

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
                HeightList = ItemsSource.Count * 74;
            }
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemsSource is not null)
            {
                HeightList = ItemsSource.Count * 74;
            }
        }

        #endregion
    }
}
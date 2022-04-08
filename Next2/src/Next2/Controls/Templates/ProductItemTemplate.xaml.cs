using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls.Templates
{
    public partial class ProductItemTemplate : Grid
    {
        public ProductItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public property --

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(ProductItemTemplate));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(ProductItemTemplate));

        public string Text
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty PriceProperty = BindableProperty.Create(
            propertyName: nameof(Price),
            returnType: typeof(float),
            declaringType: typeof(ProductItemTemplate));

        public float Price
        {
            get => (float)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(ICollection),
            typeof(ProductItemTemplate),
            default(ICollection),
            propertyChanged: (b, o, n) =>
            ((ProductItemTemplate)b).OnItemsSourcePropertyChanged((ICollection)o, (ICollection)n));

        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
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
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemsSource is not null)
            {
            }
        }

        #endregion
    }
}
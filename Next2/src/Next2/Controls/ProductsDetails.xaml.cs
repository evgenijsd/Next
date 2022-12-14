using System.Collections;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class ProductsDetails : Grid
    {
        public ProductsDetails()
        {
            InitializeComponent();
        }

        #region -- Public property --

        public static readonly BindableProperty FontSizeTitleProperty = BindableProperty.Create(
            propertyName: nameof(FontSizeTitle),
            returnType: typeof(double),
            defaultValue: (double)Application.Current.Resources["TSize_i6"],
            declaringType: typeof(ProductsDetails),
            defaultBindingMode: BindingMode.TwoWay);

        public double FontSizeTitle
        {
            get => (double)GetValue(FontSizeTitleProperty);
            set => SetValue(FontSizeTitleProperty, value);
        }

        public static readonly BindableProperty FontFamilyTitleProperty = BindableProperty.Create(
            propertyName: nameof(FontFamilyTitle),
            returnType: typeof(string),
            declaringType: typeof(ProductsDetails),
            defaultValue: "Barlow-SemiBold",
            defaultBindingMode: BindingMode.TwoWay);

        public string FontFamilyTitle
        {
            get => (string)GetValue(FontFamilyTitleProperty);
            set => SetValue(FontFamilyTitleProperty, value);
        }

        public static readonly BindableProperty FontSizePriceProperty = BindableProperty.Create(
           propertyName: nameof(FontSizePrice),
           returnType: typeof(double),
           defaultValue: (double)Application.Current.Resources["TSize_i5"],
           declaringType: typeof(ProductsDetails),
           defaultBindingMode: BindingMode.TwoWay);

        public double FontSizePrice
        {
            get => (double)GetValue(FontSizePriceProperty);
            set => SetValue(FontSizePriceProperty, value);
        }

        public static readonly BindableProperty FontFamilyPriceProperty = BindableProperty.Create(
            propertyName: nameof(FontFamilyPrice),
            returnType: typeof(string),
            declaringType: typeof(ProductsDetails),
            defaultValue: "Barlow-SemiBold",
            defaultBindingMode: BindingMode.TwoWay);

        public string FontFamilyPrice
        {
            get => (string)GetValue(FontFamilyPriceProperty);
            set => SetValue(FontFamilyPriceProperty, value);
        }

        public static readonly BindableProperty FontSizeProductProperty = BindableProperty.Create(
           propertyName: nameof(FontSizeProduct),
           returnType: typeof(double),
           defaultValue: (double)Application.Current.Resources["TSize_i5"],
           declaringType: typeof(ProductsDetails),
           defaultBindingMode: BindingMode.TwoWay);

        public double FontSizeProduct
        {
            get => (double)GetValue(FontSizeProductProperty);
            set => SetValue(FontSizeProductProperty, value);
        }

        public static readonly BindableProperty FontFamilyProductProperty = BindableProperty.Create(
            propertyName: nameof(FontFamilyProduct),
            returnType: typeof(string),
            declaringType: typeof(ProductsDetails),
            defaultValue: "Barlow-Medium",
            defaultBindingMode: BindingMode.TwoWay);

        public string FontFamilyProduct
        {
            get => (string)GetValue(FontFamilyProductProperty);
            set => SetValue(FontFamilyProductProperty, value);
        }

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(ICollection),
            typeof(ProductsDetails),
            default(ICollection));

        public ICollection ItemsSource
        {
            get => (ICollection)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        #endregion
    }
}
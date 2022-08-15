using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls.Templates
{
    public partial class TableItemTemplate : StackLayout
    {
        public TableItemTemplate()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TableNumberProperty = BindableProperty.Create(
            propertyName: nameof(TableNumber),
            returnType: typeof(int),
            declaringType: typeof(TableItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public int TableNumber
        {
            get => (int)GetValue(TableNumberProperty);
            set => SetValue(TableNumberProperty, value);
        }

        public static readonly BindableProperty ItemHeightRequestProperty = BindableProperty.Create(
            propertyName: nameof(ItemHeightRequestProperty),
            returnType: typeof(double),
            defaultValue: 21d,
            declaringType: typeof(TableItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public double ItemHeightRequest
        {
            get => (double)GetValue(ItemHeightRequestProperty);
            set => SetValue(ItemHeightRequestProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(TableItemTemplate),
            defaultValue: 14d,
            defaultBindingMode: BindingMode.OneWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls.Templates
{
    public partial class TextItemTemplate : StackLayout
    {
        public TextItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(FormattedString),
            declaringType: typeof(TextItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public FormattedString Text
        {
            get => (FormattedString)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty ItemHeightRequestProperty = BindableProperty.Create(
            propertyName: nameof(ItemHeightRequestProperty),
            returnType: typeof(double),
            defaultValue: 21d,
            declaringType: typeof(TextItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public double ItemHeightRequest
        {
            get => (double)GetValue(ItemHeightRequestProperty);
            set => SetValue(ItemHeightRequestProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(TextItemTemplate),
            defaultValue: 14d,
            defaultBindingMode: BindingMode.OneWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        #endregion
    }
}
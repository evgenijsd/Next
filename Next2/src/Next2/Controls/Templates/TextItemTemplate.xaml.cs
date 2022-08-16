using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public static readonly BindableProperty TextMarginProperty = BindableProperty.Create(
            propertyName: nameof(TextMargin),
            returnType: typeof(Thickness),
            defaultValue: new Thickness(14d, 0d),
            declaringType: typeof(TextItemTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public Thickness TextMargin
        {
            get => (Thickness)GetValue(TextMarginProperty);
            set => SetValue(TextMarginProperty, value);
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
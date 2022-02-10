using Next2.Resources.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class CustomButtonOrderTab : Grid
    {
        public CustomButtonOrderTab()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CRadiusProperty = BindableProperty.Create(
            propertyName: nameof(CRadius),
            returnType: typeof(CornerRadius),
            declaringType: typeof(CustomButtonOrderTab),
            /*defaultValue: (0, 0, 0, 0),*/
            defaultBindingMode: BindingMode.TwoWay);

        public CornerRadius CRadius
        {
            get => (CornerRadius)GetValue(CRadiusProperty);
            set => SetValue(CRadiusProperty, value);
        }

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            propertyName: nameof(IsSelected),
            returnType: typeof(bool),
            declaringType: typeof(CustomButtonOrderTab),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly BindableProperty IsRightProperty = BindableProperty.Create(
            propertyName: nameof(IsRight),
            returnType: typeof(bool),
            declaringType: typeof(CustomButtonOrderTab),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsRight
        {
            get => (bool)GetValue(IsRightProperty);
            set => SetValue(IsRightProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(CustomButton),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
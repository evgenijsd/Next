using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class CustomButton : Grid
    {
        public CustomButton()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
    propertyName: nameof(CornerRadius),
    returnType: typeof(float),
    declaringType: typeof(CustomButton),
    defaultValue: 0F,
    defaultBindingMode: BindingMode.TwoWay);

        public float CornerRadius
        {
            get => (float)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
    }
}
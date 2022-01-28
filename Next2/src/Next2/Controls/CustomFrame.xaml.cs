using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class CustomFrame : Frame
    {
        public CustomFrame()
        {
            InitializeComponent();
        }

        #region -- Public properties

        public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(
            propertyName: nameof(BorderWidth),
            returnType: typeof(float),
            declaringType: typeof(CustomFrame),
            defaultBindingMode: BindingMode.TwoWay);

        public float BorderWidth
        {
            get => (float)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }

        #endregion
    }
}
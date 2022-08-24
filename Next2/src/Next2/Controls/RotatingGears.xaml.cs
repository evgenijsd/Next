using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class RotatingGears : Grid
    {
        public RotatingGears()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IsAnimateProperty = BindableProperty.Create(
            propertyName: nameof(IsAnimate),
            returnType: typeof(bool),
            declaringType: typeof(RotatingGears),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsAnimate
        {
            get => (bool)GetValue(IsAnimateProperty);
            set => SetValue(IsAnimateProperty, value);
        }

        #endregion
    }
}
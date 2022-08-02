using Xamarin.Forms;

namespace Next2.Controls
{
    public class CustomScrollView : ScrollView
    {
        #region -- Public properties --

        public static readonly BindableProperty IsBouncesProperty = BindableProperty.Create(
            propertyName: nameof(IsBounces),
            returnType: typeof(bool),
            declaringType: typeof(CustomScrollView),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsBounces
        {
            get => (bool)GetValue(IsBouncesProperty);
            set => SetValue(IsBouncesProperty, value);
        }

        #endregion
    }
}
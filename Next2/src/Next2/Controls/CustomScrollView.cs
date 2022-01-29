using Xamarin.Forms;

namespace InterTwitter.Controls
{
    public class CustomScrollView : ScrollView
    {
        #region -- Public properties --

        public static readonly BindableProperty BouncesProperty = BindableProperty.Create(
            propertyName: nameof(IsBouncesVisible),
            returnType: typeof(bool),
            declaringType: typeof(CustomScrollView),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsBouncesVisible
        {
            get => (bool)GetValue(BouncesProperty);
            set => SetValue(BouncesProperty, value);
        }

        #endregion
    }
}
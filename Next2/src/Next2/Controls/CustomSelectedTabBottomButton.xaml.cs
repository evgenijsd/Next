using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class CustomSelectedTabBottomButton : StackLayout
    {
        public CustomSelectedTabBottomButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            propertyName: nameof(IsSelected),
            returnType: typeof(bool),
            declaringType: typeof(CustomSelectedTabBottomButton));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly BindableProperty IsUpperLineVisibleProperty = BindableProperty.Create(
           propertyName: nameof(IsUpperLineVisible),
           returnType: typeof(bool),
           declaringType: typeof(CustomSelectedTabBottomButton));

        public bool IsUpperLineVisible
        {
            get => (bool)GetValue(IsUpperLineVisibleProperty);
            set => SetValue(IsUpperLineVisibleProperty, value);
        }

        public static readonly BindableProperty IsLowerLineVisibleProperty = BindableProperty.Create(
          propertyName: nameof(IsLowerLineVisible),
          returnType: typeof(bool),
          declaringType: typeof(CustomSelectedTabBottomButton));

        public bool IsLowerLineVisible
        {
            get => (bool)GetValue(IsLowerLineVisibleProperty);
            set => SetValue(IsLowerLineVisibleProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(CustomSelectedTabBottomButton));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion
    }
}
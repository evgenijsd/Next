using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class MenuItemTemplate : StackLayout
    {
        public MenuItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            propertyName: nameof(IsSelected),
            returnType: typeof(bool),
            declaringType: typeof(MenuItemTemplate));

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly BindableProperty ImagePathProperty = BindableProperty.Create(
            propertyName: nameof(ImagePath),
            returnType: typeof(string),
            declaringType: typeof(MenuItemTemplate));

        public string ImagePath
        {
            get => (string)GetValue(ImagePathProperty);
            set => SetValue(ImagePathProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(MenuItemTemplate));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(float),
            declaringType: typeof(MenuItemTemplate),
            defaultValue: 12f,
            defaultBindingMode: BindingMode.TwoWay);

        public float FontSize
        {
            get => (float)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            declaringType: typeof(MenuItemTemplate),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        #endregion
    }
}
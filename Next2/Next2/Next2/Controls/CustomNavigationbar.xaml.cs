using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class CustomNavigationbar : ContentView
    {
        public CustomNavigationbar()
        {
            InitializeComponent();
        }

        #region -- Public properties --
        public static readonly BindableProperty RightButtonImageSourceProperty = BindableProperty.Create(
            propertyName: nameof(RightButtonImageSource),
            returnType: typeof(string),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public string RightButtonImageSource
        {
            get => (string)GetValue(RightButtonImageSourceProperty);
            set => SetValue(RightButtonImageSourceProperty, value);
        }

        public static readonly BindableProperty RightUserButtonCommandProperty = BindableProperty.Create(
            propertyName: nameof(RightUserButtonCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand RightUserButtonCommand
        {
            get => (ICommand)GetValue(RightUserButtonCommandProperty);
            set => SetValue(RightUserButtonCommandProperty, value);
        }

        public static readonly BindableProperty LeftButtonImageSourceProperty = BindableProperty.Create(
            propertyName: nameof(LeftButtonImageSource),
            returnType: typeof(string),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public string LeftButtonImageSource
        {
            get => (string)GetValue(LeftButtonImageSourceProperty);
            set => SetValue(LeftButtonImageSourceProperty, value);
        }

        public static readonly BindableProperty LeftAddButtonCommandProperty = BindableProperty.Create(
            propertyName: nameof(LeftAddButtonCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand LeftAddButtonCommand
        {
            get => (ICommand)GetValue(LeftAddButtonCommandProperty);
            set => SetValue(LeftAddButtonCommandProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly BindableProperty TitleColorProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(Color),
            declaringType: typeof(CustomNavigationbar),
            defaultBindingMode: BindingMode.TwoWay);

        public Color TitleColor
        {
            get => (Color)GetValue(TitleColorProperty);
            set => SetValue(TitleColorProperty, value);
        }

        #endregion
    }
}
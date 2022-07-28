using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class ExpanderHeaderTemplate : StackLayout
    {
        public ExpanderHeaderTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(Title),
            returnType: typeof(string),
            declaringType: typeof(ExpanderHeaderTemplate));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion
    }
}
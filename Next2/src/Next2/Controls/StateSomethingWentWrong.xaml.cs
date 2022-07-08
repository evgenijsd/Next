using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class StateSomethingWentWrong : RefreshView
    {
        public StateSomethingWentWrong()
        {
            InitializeComponent();
        }

        #region -- Public Properties --

        public static readonly BindableProperty LayoutStateProperty = BindableProperty.Create(
            propertyName: nameof(LayoutState),
            returnType: typeof(LayoutState),
            defaultValue: LayoutState.Loading,
            declaringType: typeof(StateSomethingWentWrong));

        public LayoutState LayoutState
        {
            get => (LayoutState)GetValue(LayoutStateProperty);
            set => SetValue(LayoutStateProperty, value);
        }

        #endregion
    }
}
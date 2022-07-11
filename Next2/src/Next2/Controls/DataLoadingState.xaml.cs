using Next2.Enums;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class DataLoadingState : RefreshView
    {
        public DataLoadingState()
        {
            InitializeComponent();
        }

        #region -- Public Properties --

        public static readonly BindableProperty LayoutStateProperty = BindableProperty.Create(
            propertyName: nameof(LayoutState),
            returnType: typeof(EStateLoad),
            defaultValue: EStateLoad.Loading,
            declaringType: typeof(DataLoadingState));

        public EStateLoad LayoutState
        {
            get => (EStateLoad)GetValue(LayoutStateProperty);
            set => SetValue(LayoutStateProperty, value);
        }

        #endregion
    }
}
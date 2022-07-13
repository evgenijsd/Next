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

        #region -- Public properties --

        public static readonly BindableProperty LayoutStateProperty = BindableProperty.Create(
            propertyName: nameof(LayoutState),
            returnType: typeof(ELoadingState),
            defaultValue: ELoadingState.InProgress,
            declaringType: typeof(DataLoadingState));

        public ELoadingState LayoutState
        {
            get => (ELoadingState)GetValue(LayoutStateProperty);
            set => SetValue(LayoutStateProperty, value);
        }

        #endregion
    }
}
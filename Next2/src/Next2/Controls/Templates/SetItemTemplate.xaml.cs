using Next2.Models;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class SetItemTemplate : Frame
    {
        public SetItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty SourceSetProperty = BindableProperty.Create(
            propertyName: nameof(SourceSet),
            returnType: typeof(SetModel),
            defaultValue: default(SetModel),
            declaringType: typeof(SetItemTemplate));

        public SetModel SourceSet
        {
            get => (SetModel)GetValue(SourceSetProperty);
            set => SetValue(SourceSetProperty, value);
        }

        #endregion
    }
}
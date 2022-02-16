using Next2.Models;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class PortionItemTemplate : StackLayout
    {
        public PortionItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty BindableLayoutProperty = BindableProperty.Create(
            propertyName: nameof(BindableLayout),
            returnType: typeof(PortionModel),
            declaringType: typeof(PortionItemTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public PortionModel? BindableLayout
        {
            get => (PortionModel)GetValue(BindableLayoutProperty);
            set => SetValue(BindableLayoutProperty, value);
        }

        public static readonly BindableProperty StateProperty = BindableProperty.Create(
            propertyName: nameof(State),
            returnType: typeof(PortionModel),
            declaringType: typeof(PortionItemTemplate));

        public PortionModel State
        {
            get => (PortionModel)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public static readonly BindableProperty IsToggleProperty = BindableProperty.Create(
            propertyName: nameof(IsToggle),
            returnType: typeof(bool),
            declaringType: typeof(PortionItemTemplate));

        public bool IsToggle
        {
            get => (bool)GetValue(IsToggleProperty);
            set => SetValue(IsToggleProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(State):
                    IsToggle = State?.Id == BindableLayout?.Id;

                    break;
                case nameof(BindableLayout):
                    IsToggle = State?.Id == BindableLayout?.Id;

                    break;
                case nameof(IsToggle):
                    BindableLayout = IsToggle ? State : null;

                    break;
            }
        }

        #endregion
    }
}
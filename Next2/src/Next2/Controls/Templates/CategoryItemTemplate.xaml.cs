using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class CategoryItemTemplate : Frame
    {
        public CategoryItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            propertyName: nameof(IsSelected),
            returnType: typeof(bool),
            declaringType: typeof(CategoryItemTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public static readonly BindableProperty StateProperty = BindableProperty.Create(
            propertyName: nameof(State),
            returnType: typeof(object),
            declaringType: typeof(CategoryItemTemplate));

        public object State
        {
            get => GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public static readonly BindableProperty BindableStateProperty = BindableProperty.Create(
            propertyName: nameof(BindableState),
            returnType: typeof(object),
            declaringType: typeof(CategoryItemTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public object BindableState
        {
            get => GetValue(BindableStateProperty);
            set => SetValue(BindableStateProperty, value);
        }

        public static readonly BindableProperty IsSelectVisualEffectChangeProperty = BindableProperty.Create(
            propertyName: nameof(IsSelectVisualEffectChange),
            returnType: typeof(bool),
            defaultValue: true,
            declaringType: typeof(CategoryItemTemplate));

        public bool IsSelectVisualEffectChange
        {
            get => (bool)GetValue(IsSelectVisualEffectChangeProperty);
            set => SetValue(IsSelectVisualEffectChangeProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(CategoryItemTemplate));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(float),
            declaringType: typeof(CategoryItemTemplate),
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
            declaringType: typeof(CategoryItemTemplate),
            defaultValue: string.Empty);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public ICommand TapCommand => new AsyncCommand(OnTapCommandAsync);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(State):
                case nameof(BindableState):

                    if (IsSelectVisualEffectChange)
                    {
                        IsSelected = BindableState?.ToString() == State?.ToString();
                    }

                    break;
            }
        }

        #endregion

        #region -- Private methods --

        private Task OnTapCommandAsync()
        {
            BindableState = State;

            if (IsSelectVisualEffectChange)
            {
                IsSelected = BindableState?.ToString() == State?.ToString();
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
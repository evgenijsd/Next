using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class Toggle : Grid
    {
        private double _valueX;

        public Toggle()
        {
            InitializeComponent();

            thumbFrame.TranslationX = ThumbOffset;
        }

        #region -- Public properties --

        public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
            propertyName: nameof(IsToggled),
            returnType: typeof(bool),
            declaringType: typeof(Toggle),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsToggled
        {
            get => (bool)GetValue(IsToggledProperty);
            set => SetValue(IsToggledProperty, value);
        }

        public static readonly BindableProperty CanTurnOffProperty = BindableProperty.Create(
            propertyName: nameof(CanTurnOff),
            returnType: typeof(bool),
            defaultValue: true,
            declaringType: typeof(Toggle),
            defaultBindingMode: BindingMode.OneWay);

        public bool CanTurnOff
        {
            get => (bool)GetValue(CanTurnOffProperty);
            set => SetValue(CanTurnOffProperty, value);
        }

        public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(
            propertyName: nameof(ThumbColor),
            returnType: typeof(Color),
            declaringType: typeof(Toggle),
            defaultBindingMode: BindingMode.OneWay);

        public Color ThumbColor
        {
            get => (Color)GetValue(ThumbColorProperty);
            set => SetValue(ThumbColorProperty, value);
        }

        public static readonly BindableProperty ThumbSizeProperty = BindableProperty.Create(
            propertyName: nameof(ThumbSize),
            returnType: typeof(double),
            declaringType: typeof(Toggle),
            defaultValue: 21d,
            defaultBindingMode: BindingMode.OneWay);

        public double ThumbSize
        {
            get => (double)GetValue(ThumbSizeProperty);
            set => SetValue(ThumbSizeProperty, value);
        }

        public static readonly BindableProperty ThumbOffsetProperty = BindableProperty.Create(
            propertyName: nameof(ThumbOffset),
            returnType: typeof(double),
            declaringType: typeof(Toggle),
            defaultValue: 2d,
            defaultBindingMode: BindingMode.OneWay);

        public double ThumbOffset
        {
            get => (double)GetValue(ThumbOffsetProperty);
            set => SetValue(ThumbOffsetProperty, value);
        }

        public static readonly BindableProperty OnColorProperty = BindableProperty.Create(
            propertyName: nameof(OnColor),
            returnType: typeof(Color),
            declaringType: typeof(Toggle),
            defaultBindingMode: BindingMode.OneWay);

        public Color OnColor
        {
            get => (Color)GetValue(OnColorProperty);
            set => SetValue(OnColorProperty, value);
        }

        public static readonly BindableProperty ChangingToggleCommandProperty = BindableProperty.Create(
            propertyName: nameof(ChangingToggleCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(Toggle),
            defaultBindingMode: BindingMode.OneWay);

        public ICommand ChangingToggleCommand
        {
            get => (ICommand)GetValue(ChangingToggleCommandProperty);
            set => SetValue(ChangingToggleCommandProperty, value);
        }

        public static readonly BindableProperty ChangingToggleCommandParameterProperty = BindableProperty.Create(
            propertyName: nameof(ChangingToggleCommandParameter),
            returnType: typeof(object),
            declaringType: typeof(Toggle),
            defaultBindingMode: BindingMode.OneWay);

        public object ChangingToggleCommandParameter
        {
            get => (object)GetValue(ChangingToggleCommandParameterProperty);
            set => SetValue(ChangingToggleCommandParameterProperty, value);
        }

        private ICommand _tapCommand;
        public ICommand TapCommand => _tapCommand ??= new AsyncCommand(OnTapCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsToggled))
            {
                StartAnimationAsync();
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task StartAnimationAsync()
        {
            var x = IsToggled
                ? thumbFrame.X + trackFrame.WidthRequest - thumbFrame.WidthRequest - ThumbOffset
                : thumbFrame.X + ThumbOffset;

            await thumbFrame.TranslateTo(x, 0, 100, Easing.CubicInOut);
        }

        private Task OnTapCommandAsync()
        {
            if (IsEnabled && (!IsToggled || CanTurnOff))
            {
                IsToggled = !IsToggled;
                ChangingToggleCommand?.Execute(ChangingToggleCommandParameter);
            }

            return Task.CompletedTask;
        }

        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (IsEnabled)
            {
                var x = e.TotalX;

                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        if (x < _valueX && CanTurnOff)
                        {
                            IsToggled = false;
                            ChangingToggleCommand?.Execute(ChangingToggleCommandParameter);
                        }
                        else if (x > _valueX)
                        {
                            IsToggled = true;
                            ChangingToggleCommand?.Execute(ChangingToggleCommandParameter);
                        }

                        break;
                    case GestureStatus.Completed:
                        _valueX = x;
                        break;
                }
            }
        }

        #endregion
    }
}
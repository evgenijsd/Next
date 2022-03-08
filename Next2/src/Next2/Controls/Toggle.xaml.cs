using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class Toggle : StackLayout
    {
        private double _valueX;

        public Toggle()
        {
            InitializeComponent();
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
            declaringType: typeof(Toggle));

        public bool CanTurnOff
        {
            get => (bool)GetValue(CanTurnOffProperty);
            set => SetValue(CanTurnOffProperty, value);
        }

        public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(
            propertyName: nameof(ThumbColor),
            returnType: typeof(Color),
            declaringType: typeof(Toggle));

        public Color ThumbColor
        {
            get => (Color)GetValue(ThumbColorProperty);
            set => SetValue(ThumbColorProperty, value);
        }

        public static readonly BindableProperty OnColorProperty = BindableProperty.Create(
            propertyName: nameof(OnColor),
            returnType: typeof(Color),
            declaringType: typeof(Toggle));

        public Color OnColor
        {
            get => (Color)GetValue(OnColorProperty);
            set => SetValue(OnColorProperty, value);
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

        #region -- Private methods --

        private async Task StartAnimationAsync()
        {
            if (IsToggled)
            {
                await runningFrame.TranslateTo(runningFrame.X + 17, 0, 100, Easing.CubicInOut);
            }
            else
            {
                await runningFrame.TranslateTo(runningFrame.X, 0, 100, Easing.CubicInOut);
            }
        }

        private async Task OnTapCommandAsync()
        {
            if (IsEnabled)
            {
                if (IsToggled)
                {
                    if (CanTurnOff)
                    {
                        IsToggled = false;
                    }
                }
                else
                {
                    IsToggled = true;
                }
            }
        }

        private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (IsEnabled)
            {
                var x = e.TotalX;

                switch (e.StatusType)
                {
                    case GestureStatus.Running:
                        if (x < _valueX)
                        {
                            if (CanTurnOff)
                            {
                                IsToggled = false;
                            }
                        }
                        else if (x > _valueX)
                        {
                            IsToggled = true;
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
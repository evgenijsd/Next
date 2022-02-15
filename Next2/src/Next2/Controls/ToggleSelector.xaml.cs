using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class ToggleSelector : StackLayout
    {
        private double _valueX;

        private bool _init;

        public ToggleSelector()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IsToggledProperty = BindableProperty.Create(
            propertyName: nameof(IsToggled),
            returnType: typeof(bool),
            declaringType: typeof(ToggleSelector),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsToggled
        {
            get => (bool)GetValue(IsToggledProperty);
            set => SetValue(IsToggledProperty, value);
        }

        public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(
            propertyName: nameof(ThumbColor),
            returnType: typeof(Color),
            declaringType: typeof(ToggleSelector),
            defaultBindingMode: BindingMode.TwoWay);

        public Color ThumbColor
        {
            get => (Color)GetValue(ThumbColorProperty);
            set => SetValue(ThumbColorProperty, value);
        }

        public static readonly BindableProperty OnColorProperty = BindableProperty.Create(
            propertyName: nameof(OnColor),
            returnType: typeof(Color),
            declaringType: typeof(ToggleSelector),
            defaultBindingMode: BindingMode.TwoWay);

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

            if (!_init && propertyName == nameof(IsToggled))
            {
                ToggleChangingAsync();
                _init = true;
            }
        }

        #endregion

        #region -- Private methods --

        private async Task ToggleChangingAsync()
        {
            if (IsToggled)
            {
                await runningFrame.TranslateTo(runningFrame.X + 23, 0, 50, Easing.CubicInOut);
            }
            else
            {
                await runningFrame.TranslateTo(runningFrame.X, 0, 50, Easing.CubicInOut);
            }
        }

        private async Task OnTapCommandAsync()
        {
            if (IsEnabled)
            {
                IsToggled = !IsToggled;

                await ToggleChangingAsync();
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
                            IsToggled = false;
                        }
                        else if (x > _valueX)
                        {
                            IsToggled = true;
                        }

                        ToggleChangingAsync();

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
using System.Threading.Tasks;
using System;
using Xamarin.Forms;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using System.Runtime.CompilerServices;

namespace Next2.Controls
{
    public partial class NumericKeyboard : ContentView
    {
        public NumericKeyboard()
        {
            InitializeComponent();
        }

        #region -- Public property --

        public static readonly BindableProperty ScreenKeyboardProperty = BindableProperty.Create(
            propertyName: nameof(ScreenKeyboard),
            returnType: typeof(string),
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public string ScreenKeyboard
        {
            get => (string)GetValue(ScreenKeyboardProperty);
            set => SetValue(ScreenKeyboardProperty, value);
        }

        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            propertyName: nameof(Value),
            returnType: typeof(decimal),
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public decimal Value
        {
            get => (decimal)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly BindableProperty ButtonBackgroundColorProperty = BindableProperty.Create(
            propertyName: nameof(ButtonBackgroundColor),
            returnType: typeof(Color),
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public Color ButtonBackgroundColor
        {
            get => (Color)GetValue(ButtonBackgroundColorProperty);
            set => SetValue(ButtonBackgroundColorProperty, value);
        }

        public static readonly BindableProperty ValueFormatProperty = BindableProperty.Create(
            propertyName: nameof(ValueFormat),
            returnType: typeof(string),
            defaultValue: "{0}",
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public string ValueFormat
        {
            get => (string)GetValue(ValueFormatProperty);
            set => SetValue(ValueFormatProperty, value);
        }

        public static readonly BindableProperty MaxValueProperty = BindableProperty.Create(
            propertyName: nameof(MaxValue),
            returnType: typeof(decimal),
            defaultValue: 1000m,
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public decimal MaxValue
        {
            get => (decimal)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly BindableProperty IsKeyboardTypedProperty = BindableProperty.Create(
            propertyName: nameof(IsKeyboardTyped),
            returnType: typeof(bool),
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsKeyboardTyped
        {
            get => (bool)GetValue(IsKeyboardTypedProperty);
            set => SetValue(IsKeyboardTypedProperty, value);
        }

        public static readonly BindableProperty IsKeyboardEnabledProperty = BindableProperty.Create(
            propertyName: nameof(IsKeyboardEnabled),
            returnType: typeof(bool),
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsKeyboardEnabled
        {
            get => (bool)GetValue(IsKeyboardEnabledProperty);
            set => SetValue(IsKeyboardEnabledProperty, value);
        }

        public static readonly BindableProperty IsTextRightToLeftProperty = BindableProperty.Create(
            propertyName: nameof(IsTextRightToLeft),
            returnType: typeof(bool),
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsTextRightToLeft
        {
            get => (bool)GetValue(IsTextRightToLeftProperty);
            set => SetValue(IsTextRightToLeftProperty, value);
        }

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
            propertyName: nameof(Placeholder),
            returnType: typeof(string),
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        private ICommand _buttonTapCommand;
        public ICommand ButtonTapCommand => _buttonTapCommand ??= new AsyncCommand<object>(OnButtonTapCommandAsync);

        private ICommand _buttonClearTapCommand;
        public ICommand ButtonClearTapCommand => _buttonClearTapCommand ??= new AsyncCommand<object>(OnButtonClearTapCommandAsync);

        #endregion

        #region -- Ovverides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Value))
            {
                ScreenKeyboard = string.Format(ValueFormat, Value);
            }
        }

        #endregion

        #region -- Private helpers --

        private async Task OnButtonTapCommandAsync(object? sender)
        {
            if (sender is string str && str is not null && IsKeyboardEnabled)
            {
                if (!IsKeyboardTyped)
                {
                    IsKeyboardTyped = true;
                }

                if (decimal.TryParse(str, out decimal enteredDigit))
                {
                    var tmp = ((Value * 1000m) + enteredDigit) / 100m;

                    if (tmp <= MaxValue)
                    {
                        Value = tmp;
                    }
                }

                ScreenKeyboard = string.Format(ValueFormat, Value);
            }
        }

        private async Task OnButtonClearTapCommandAsync(object? arg)
        {
            ScreenKeyboard = Placeholder;
            Value = 0m;
            IsKeyboardTyped = false;
        }

        #endregion
    }
}
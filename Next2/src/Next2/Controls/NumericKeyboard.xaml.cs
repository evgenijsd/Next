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

        public static readonly BindableProperty BackgroundColorButtonProperty = BindableProperty.Create(
            propertyName: nameof(BackgroundColorButton),
            returnType: typeof(Color),
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public Color BackgroundColorButton
        {
            get => (Color)GetValue(BackgroundColorButtonProperty);
            set => SetValue(BackgroundColorButtonProperty, value);
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

        public static readonly BindableProperty IsKeyBoardTypedProperty = BindableProperty.Create(
            propertyName: nameof(IsKeyBoardTyped),
            returnType: typeof(bool),
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsKeyBoardTyped
        {
            get => (bool)GetValue(IsKeyBoardTypedProperty);
            set => SetValue(IsKeyBoardTypedProperty, value);
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

        public static readonly BindableProperty PlaceHolderProperty = BindableProperty.Create(
            propertyName: nameof(PlaceHolder),
            returnType: typeof(string),
            declaringType: typeof(NumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public string PlaceHolder
        {
            get => (string)GetValue(PlaceHolderProperty);
            set => SetValue(PlaceHolderProperty, value);
        }

        private ICommand _buttonTapCommand;
        public ICommand ButtonTapCommand => _buttonTapCommand ??= new AsyncCommand<object>(OnTabAsync);

        private ICommand _buttonClearTapCommand;
        public ICommand ButtonClearTapCommand => _buttonClearTapCommand ??= new AsyncCommand<object>(OnTabClearAsync);

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

        private async Task OnTabAsync(object? sender)
        {
            if (sender is string str && str is not null)
            {
                if (!IsKeyBoardTyped)
                {
                    IsKeyBoardTyped = true;
                }

                if (Value < MaxValue)
                {
                    if (decimal.TryParse(str, out decimal tmp))
                    {
                        Value *= 1000m;
                        Value = (Value + tmp) / 100m;
                    }
                }

                ScreenKeyboard = string.Format(ValueFormat, Value);
            }
        }

        private async Task OnTabClearAsync(object? arg)
        {
            ScreenKeyboard = PlaceHolder;
            Value = 0m;
            IsKeyBoardTyped = false;
        }

        #endregion
    }
}
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class CustomNumericKeyboardTemplate : ContentView
    {
        private double _numericValue;

        public CustomNumericKeyboardTemplate()
        {
            InitializeComponent();
        }

        #region -- Public property --

        public static readonly BindableProperty ScreenKeyboardProperty = BindableProperty.Create(
            propertyName: nameof(ScreenKeyboard),
            returnType: typeof(string),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public string ScreenKeyboard
        {
            get => (string)GetValue(ScreenKeyboardProperty);
            set => SetValue(ScreenKeyboardProperty, value);
        }

        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            propertyName: nameof(Value),
            returnType: typeof(string),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly BindableProperty BackgroundColorButtonProperty = BindableProperty.Create(
            propertyName: nameof(BackgroundColorButton),
            returnType: typeof(Color),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public Color BackgroundColorButton
        {
            get => (Color)GetValue(BackgroundColorButtonProperty);
            set => SetValue(BackgroundColorButtonProperty, value);
        }

        public static readonly BindableProperty ValueFormatProperty = BindableProperty.Create(
            propertyName: nameof(ValueFormat),
            returnType: typeof(string),
            defaultValue: "{0}",
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string ValueFormat
        {
            get => (string)GetValue(ValueFormatProperty);
            set => SetValue(ValueFormatProperty, value);
        }

        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
            propertyName: nameof(MaxLength),
            returnType: typeof(int),
            defaultValue: 6,
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public static readonly BindableProperty IsKeyBoardTypedProperty = BindableProperty.Create(
            propertyName: nameof(IsKeyBoardTyped),
            returnType: typeof(bool),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsKeyBoardTyped
        {
            get => (bool)GetValue(IsKeyBoardTypedProperty);
            set => SetValue(IsKeyBoardTypedProperty, value);
        }

        public static readonly BindableProperty IsNumericModeProperty = BindableProperty.Create(
            propertyName: nameof(IsNumericMode),
            returnType: typeof(bool),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsNumericMode
        {
            get => (bool)GetValue(IsNumericModeProperty);
            set => SetValue(IsNumericModeProperty, value);
        }

        public static readonly BindableProperty IsTextRightToLeftProperty = BindableProperty.Create(
            propertyName: nameof(IsTextRightToLeft),
            returnType: typeof(bool),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsTextRightToLeft
        {
            get => (bool)GetValue(IsTextRightToLeftProperty);
            set => SetValue(IsTextRightToLeftProperty, value);
        }

        public static readonly BindableProperty IsUserLogInProperty = BindableProperty.Create(
            propertyName: nameof(IsUserLogIn),
            returnType: typeof(bool),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsUserLogIn
        {
            get => (bool)GetValue(IsUserLogInProperty);
            set => SetValue(IsUserLogInProperty, value);
        }

        public static readonly BindableProperty PlaceHolderProperty = BindableProperty.Create(
            propertyName: nameof(PlaceHolder),
            returnType: typeof(string),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string PlaceHolder
        {
            get => (string)GetValue(PlaceHolderProperty);
            set => SetValue(PlaceHolderProperty, value);
        }

        public static readonly BindableProperty IsErrorNotificationVisibleProperty = BindableProperty.Create(
           propertyName: nameof(IsErrorNotificationVisible),
           returnType: typeof(bool),
           declaringType: typeof(CustomNumericKeyboardTemplate),
           defaultBindingMode: BindingMode.TwoWay);

        public bool IsErrorNotificationVisible
        {
            get => (bool)GetValue(IsErrorNotificationVisibleProperty);
            set => SetValue(IsErrorNotificationVisibleProperty, value);
        }

        public static readonly BindableProperty ErrorPlaceHolderProperty = BindableProperty.Create(
            propertyName: nameof(ErrorPlaceHolder),
            returnType: typeof(string),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public string ErrorPlaceHolder
        {
            get => (string)GetValue(ErrorPlaceHolderProperty);
            set => SetValue(ErrorPlaceHolderProperty, value);
        }

        public static readonly BindableProperty IsErrorPlaceHolderVisibleProperty = BindableProperty.Create(
            propertyName: nameof(IsErrorPlaceHolderVisible),
            returnType: typeof(bool),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsErrorPlaceHolderVisible
        {
            get => (bool)GetValue(IsErrorPlaceHolderVisibleProperty);
            set => SetValue(IsErrorPlaceHolderVisibleProperty, value);
        }

        private ICommand _buttonTapCommand;
        public ICommand ButtonTapCommand => _buttonTapCommand ??= new AsyncCommand<object>(OnTabAsync);

        private ICommand _buttonClearTapCommand;
        public ICommand ButtonClearTapCommand => _buttonClearTapCommand ??= new AsyncCommand<object>(OnTabClearAsync);

        #endregion

        #region -- Ovverides --

        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);

            if (propertyName == nameof(IsUserLogIn))
            {
                _numericValue = 0;
                Value = PlaceHolder;
                ScreenKeyboard = PlaceHolder;

                IsKeyBoardTyped = false;
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

                    _numericValue = 0;
                    Value = string.Empty;
                }

                if (Value.Length < MaxLength)
                {
                    if (IsNumericMode)
                    {
                        if (double.TryParse(str, out double tmp))
                        {
                            _numericValue *= 10;
                            _numericValue += tmp / 100;

                            if (_numericValue > 0)
                            {
                                Value += str;
                            }
                        }
                    }
                    else
                    {
                        Value += str;
                    }
                }

                ScreenKeyboard = IsNumericMode ? string.Format(ValueFormat, _numericValue) : string.Format(ValueFormat, Value);
            }
        }

        private async Task OnTabClearAsync(object? arg)
        {
            ScreenKeyboard = PlaceHolder;
            Value = string.Empty;
            _numericValue = 0;
            IsKeyBoardTyped = false;
            IsErrorNotificationVisible = false;
        }

        #endregion
    }
}
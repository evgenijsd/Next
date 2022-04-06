using System.Threading.Tasks;
using System;
using Xamarin.Forms;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using System.Runtime.CompilerServices;

namespace Next2.Controls.Templates
{
    public partial class CustomNumericKeyboardTemplate : ContentView
    {
        private string _value;

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

        public static readonly BindableProperty BackgroundColorButtonProperty = BindableProperty.Create(
            propertyName: nameof(BackgroundColorButton),
            returnType: typeof(Color),
            declaringType: typeof(CustomNumericKeyboardTemplate),
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
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.TwoWay);

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
            defaultBindingMode: BindingMode.TwoWay);

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

        public static readonly BindableProperty IsUserLogInProperty = BindableProperty.Create(
            propertyName: nameof(IsUserLogIn),
            returnType: typeof(bool),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsUserLogIn
        {
            get => (bool)GetValue(IsUserLogInProperty);
            set => SetValue(IsUserLogInProperty, value);
        }

        public static readonly BindableProperty PlaceHolderProperty = BindableProperty.Create(
            propertyName: nameof(PlaceHolder),
            returnType: typeof(string),
            declaringType: typeof(CustomNumericKeyboardTemplate),
            defaultBindingMode: BindingMode.TwoWay);

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

        private ICommand _buttonTapCommand;
        public ICommand ButtonTapCommand => _buttonTapCommand ??= new AsyncCommand<object>(OnTabAsync);

        private ICommand _buttonClearTapCommand;
        public ICommand ButtonClearTapCommand => _buttonClearTapCommand ??= new AsyncCommand<object>(OnTabClearAsync);

        public static readonly BindableProperty ApplyCommandProperty = BindableProperty.Create(
         propertyName: nameof(ApplyCommand),
         returnType: typeof(ICommand),
         declaringType: typeof(CustomNumericKeyboardTemplate),
         defaultBindingMode: BindingMode.TwoWay);

        public ICommand ApplyCommand
        {
            get => (ICommand)GetValue(ApplyCommandProperty);
            set => SetValue(ApplyCommandProperty, value);
        }

        #endregion

        #region -- Ovverides --

        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanging(propertyName);

            if (propertyName == nameof(IsUserLogIn))
            {
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
                if (IsKeyBoardTyped)
                {
                    if (_value.Length < MaxLength)
                    {
                        _value += str;
                        ScreenKeyboard = string.Format(ValueFormat, _value);
                    }
                }
                else
                {
                    IsKeyBoardTyped = true;
                    _value = str;
                    ScreenKeyboard = string.Format(ValueFormat, _value);
                }
            }
        }

        private async Task OnTabClearAsync(object? arg)
        {
            _value = string.Empty;
            ScreenKeyboard = PlaceHolder;
            IsKeyBoardTyped = false;
            IsErrorNotificationVisible = false;
        }

        #endregion
    }
}
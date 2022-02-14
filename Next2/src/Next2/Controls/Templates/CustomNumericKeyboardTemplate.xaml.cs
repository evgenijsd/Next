using System.Threading.Tasks;
using System;
using Xamarin.Forms;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.Controls.Templates
{
    public partial class CustomNumericKeyboardTemplate : ContentView
    {
        public CustomNumericKeyboardTemplate()
        {
            InitializeComponent();
            IsKeyBoardTyped = false;
        }

        #region -- Public property --

        public bool IsKeyBoardTyped { get; set; }

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

        private ICommand _ButtonTapCommand;
        public ICommand ButtonTapCommand => _ButtonTapCommand ??= new AsyncCommand<object>(OnTabAsync);

        private ICommand _ButtonClearTapCommand;
        public ICommand ButtonClearTapCommand => _ButtonClearTapCommand ??= new AsyncCommand<object>(OnTabClearAsync);

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

        #region -- Private helpers --

        private async Task OnTabAsync(object? sender)
        {
            if (sender is string str && str is not null)
            {
                if (IsKeyBoardTyped)
                {
                    if (ScreenKeyboard.Length <= 5)
                    {
                        ScreenKeyboard += str;
                    }
                }
                else
                {
                    IsKeyBoardTyped = true;
                    ScreenKeyboard = str;
                }
            }
        }

        private async Task OnTabClearAsync(object? arg)
        {
            ScreenKeyboard = PlaceHolder;
            IsKeyBoardTyped = false;
            IsErrorNotificationVisible = false;
        }

        #endregion
    }
}
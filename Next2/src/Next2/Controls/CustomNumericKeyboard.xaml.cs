using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class CustomNumericKeyboard : ContentView
    {
        public CustomNumericKeyboard()
        {
            InitializeComponent();
        }

        private bool _startTyping;

        #region -- Public property --

        public bool IsKeyBoardTyped { get; set; }

        public static readonly BindableProperty ScreenKeyboardProperty = BindableProperty.Create(
            propertyName: nameof(ScreenKeyboard),
            returnType: typeof(string),
            declaringType: typeof(CustomNumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public string ScreenKeyboard
        {
            get => (string)GetValue(ScreenKeyboardProperty);
            set => SetValue(ScreenKeyboardProperty, value);
        }

        public static readonly BindableProperty PlaceHolderProperty = BindableProperty.Create(
            propertyName: nameof(PlaceHolder),
            returnType: typeof(string),
            declaringType: typeof(CustomNumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public string PlaceHolder
        {
            get => (string)GetValue(PlaceHolderProperty);
            set => SetValue(PlaceHolderProperty, value);
        }

        public static readonly BindableProperty ErrorNotificationProperty = BindableProperty.Create(
           propertyName: nameof(ErrorNotification),
           returnType: typeof(bool),
           declaringType: typeof(CustomNumericKeyboard),
           defaultBindingMode: BindingMode.TwoWay);

        public bool ErrorNotification
        {
            get => (bool)GetValue(ErrorNotificationProperty);
            set => SetValue(ErrorNotificationProperty, value);
        }

        private ICommand _ButtonTabCommand;
        public ICommand ButtonTabCommand => _ButtonTabCommand ??= new AsyncCommand<object>(OnTabAsync);

        private ICommand _ButtonClearTabCommand;
        public ICommand ButtonClearTabCommand => _ButtonClearTabCommand ??= new AsyncCommand<object>(OnTabClearAsync);

        #endregion

        #region -- Private helpers --

        private async Task OnTabAsync(object? sender)
        {
            var view = sender as Xamarin.Forms.Label;

            if (view is not null)
            {
                if (_startTyping)
                {
                    if (PlaceHolder.Length <= 5)
                    {
                        PlaceHolder += view.Text;
                    }
                }
                else
                {
                    IsKeyBoardTyped = true;
                    _startTyping = true;
                    ScreenKeyboard = view.Text;
                }
            }
        }

        private async Task OnTabClearAsync(object? arg)
        {
            ScreenKeyboard = PlaceHolder;
            _startTyping = false;
            IsKeyBoardTyped = false;
            ErrorNotification = false;
        }
        #endregion
    }
}
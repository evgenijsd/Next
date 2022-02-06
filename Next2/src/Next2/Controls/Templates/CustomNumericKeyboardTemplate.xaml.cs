using System.Threading.Tasks;
using System;
using Xamarin.Forms;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Next2.Controls.Templates
{
    public partial class CustomNumericKeyboardTemplate : ContentView
    {
        private bool _startTyping;
        public CustomNumericKeyboardTemplate()
        {
            InitializeComponent();
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

        public static readonly BindableProperty ErrorNotificationProperty = BindableProperty.Create(
           propertyName: nameof(ErrorNotification),
           returnType: typeof(bool),
           declaringType: typeof(CustomNumericKeyboardTemplate),
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
            var view = sender as Label;

            if (view is not null)
            {
                if (_startTyping)
                {
                    if (ScreenKeyboard.Length <= 5)
                    {
                        ScreenKeyboard += view.Text;
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
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

        public static readonly BindableProperty EmployeeIdProperty = BindableProperty.Create(
            propertyName: nameof(EmployeeId),
            returnType: typeof(string),
            defaultValue: "Type Employee ID",
            declaringType: typeof(CustomNumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public string EmployeeId
        {
            get => (string)GetValue(EmployeeIdProperty);
            set => SetValue(EmployeeIdProperty, value);
        }

        public static readonly BindableProperty ErrorMessageProperty = BindableProperty.Create(
           propertyName: nameof(ErrorMessage),
           returnType: typeof(bool),
           declaringType: typeof(CustomNumericKeyboard),
           defaultBindingMode: BindingMode.TwoWay);

        public bool ErrorMessage
        {
            get => (bool)GetValue(ErrorMessageProperty);
            set => SetValue(ErrorMessageProperty, value);
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
                    if (EmployeeId.Length <= 5)
                    {
                        EmployeeId += view.Text;
                    }
                }
                else
                {
                    IsKeyBoardTyped = true;
                    EmployeeId = view.Text;
                    _startTyping = true;
                }
            }
        }

        private async Task OnTabClearAsync(object? arg)
        {
            EmployeeId = "Type Employee ID";
            _startTyping = false;
            IsKeyBoardTyped = false;
            ErrorMessage = false;
        }
        #endregion
    }
}
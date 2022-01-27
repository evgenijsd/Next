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

        #region -- Public property --

        private ICommand _ButtonTabCommand;
        public ICommand ButtonTabCommand => _ButtonTabCommand ??= new AsyncCommand<object>(OnTabAsync);

        private ICommand _ButtonClearTabCommand;
        public ICommand ButtonClearTabCommand => _ButtonClearTabCommand ??= new AsyncCommand<object>(OnTabClearAsync);

        //public static readonly BindableProperty ButtonCommandProperty = BindableProperty.Create(
        //  propertyName: nameof(ButtonCommand),
        //  returnType: typeof(ICommand),
        //  declaringType: typeof(CustomNumericKeyboard),
        //  defaultValue: null,
        //  defaultBindingMode: BindingMode.TwoWay);

        //public ICommand ButtonCommand
        //{
        //    get => (ICommand)GetValue(ButtonCommandProperty);
        //    set => SetValue(ButtonCommandProperty, value);
        //}
        public static readonly BindableProperty EmployeeIdProperty = BindableProperty.Create(
            propertyName: nameof(EmployeeId),
            returnType: typeof(string),
            declaringType: typeof(CustomNumericKeyboard),
            defaultBindingMode: BindingMode.TwoWay);

        public string EmployeeId
        {
            get => (string)GetValue(EmployeeIdProperty);
            set => SetValue(EmployeeIdProperty, value);
        }

        #endregion

        #region -- Private helpers --

        private async Task OnTabAsync(object? sender)
        {
            var view = sender as Xamarin.Forms.Label;

            if (view is not null)
            {
                EmployeeId += view.Text;
            }
        }

        private async Task OnTabClearAsync(object? arg)
        {
            EmployeeId = string.Empty;
        }
        #endregion
    }
}
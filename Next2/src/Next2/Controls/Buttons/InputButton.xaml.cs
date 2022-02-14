using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls.Buttons
{
    public partial class InputButton : CustomFrame
    {
        public InputButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
         propertyName: nameof(TextColor),
         returnType: typeof(Color),
         declaringType: typeof(InputButton),
         defaultBindingMode: BindingMode.TwoWay);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
           propertyName: nameof(Text),
           returnType: typeof(string),
           declaringType: typeof(InputButton),
           defaultBindingMode: BindingMode.TwoWay);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty IsErrorNotificationVisibleProperty = BindableProperty.Create(
            propertyName: nameof(IsErrorNotificationVisibleProperty),
            returnType: typeof(bool),
            declaringType: typeof(InputButton),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsErrorNotificationVisible
        {
            get => (bool)GetValue(IsErrorNotificationVisibleProperty);
            set => SetValue(IsErrorNotificationVisibleProperty, value);
        }

        public static readonly BindableProperty IsEmployeeExistsProperty = BindableProperty.Create(
          propertyName: nameof(IsEmployeeExistsProperty),
          returnType: typeof(bool),
          declaringType: typeof(InputButton),
          defaultBindingMode: BindingMode.TwoWay);

        public bool IsEmployeeExists
        {
            get => (bool)GetValue(IsEmployeeExistsProperty);
            set => SetValue(IsEmployeeExistsProperty, value);
        }

        public static readonly BindableProperty TapGestureRecognizerCommandProperty = BindableProperty.Create(
            propertyName: nameof(TapGestureRecognizerCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(InputButton),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand TapGestureRecognizerCommand
        {
            get => (ICommand)GetValue(TapGestureRecognizerCommandProperty);
            set => SetValue(TapGestureRecognizerCommandProperty, value);
        }

        #endregion
    }
}
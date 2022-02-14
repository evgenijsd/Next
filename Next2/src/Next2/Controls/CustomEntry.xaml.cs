using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class CustomEntry : ContentView
    {
        public CustomEntry()
        {
            InitializeComponent();

            //CustomEntryLocal.Focus();
        }

        #region -- Public properties --

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(CustomEntry),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay);
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomEntry),
            defaultValue: Color.Silver,
            defaultBindingMode: BindingMode.TwoWay);
        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
            propertyName: nameof(Placeholder),
            returnType: typeof(string),
            declaringType: typeof(CustomEntry),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay);
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
            propertyName: nameof(PlaceholderColor),
            returnType: typeof(Color),
            declaringType: typeof(CustomEntry),
            defaultValue: Color.Silver,
            defaultBindingMode: BindingMode.TwoWay);
        public Color PlaceholderColor
        {
            get => (Color)GetValue(PlaceholderColorProperty);
            set => SetValue(PlaceholderColorProperty, value);
        }

        public static readonly BindableProperty ReturnCommandProperty = BindableProperty.Create(
            propertyName: nameof(ReturnCommand),
            returnType: typeof(ICommand),
            declaringType: typeof(CustomEntry),
            defaultBindingMode: BindingMode.TwoWay);
        public ICommand ReturnCommand
        {
            get => (ICommand)GetValue(ReturnCommandProperty);
            set => SetValue(ReturnCommandProperty, value);
        }

        public static readonly BindableProperty ReturnCommandParameterProperty = BindableProperty.Create(
            propertyName: nameof(ReturnCommandParameter),
            returnType: typeof(string),
            declaringType: typeof(CustomEntry),
            defaultBindingMode: BindingMode.TwoWay);
        public string ReturnCommandParameter
        {
            get => (string)GetValue(ReturnCommandParameterProperty);
            set => SetValue(ReturnCommandParameterProperty, value);
        }

        public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
            propertyName: nameof(MaxLength),
            returnType: typeof(int),
            declaringType: typeof(CustomEntry),
            defaultValue: 100,
            defaultBindingMode: BindingMode.TwoWay);
        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            declaringType: typeof(CustomEntry),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay);
        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(
            propertyName: nameof(ReturnType),
            returnType: typeof(Xamarin.Forms.ReturnType),
            declaringType: typeof(CustomEntry),
            defaultBindingMode: BindingMode.TwoWay);
        public Xamarin.Forms.ReturnType ReturnType
        {
            get => (Xamarin.Forms.ReturnType)GetValue(ReturnTypeProperty);
            set => SetValue(ReturnTypeProperty, value);
        }

        private ICommand _unfocusedCommand;
        public ICommand UnFocusedCommand => _unfocusedCommand ??= new AsyncCommand(OnUnfocusedCommandAsync);

        #endregion

        #region -- Private methods --

        private Task OnUnfocusedCommandAsync()
        {
            CustomEntryLocal.Focus();

            return Task.CompletedTask;
        }

        #endregion
    }
}
﻿using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Buttons
{
    public partial class BorderButton : CustomFrame
    {
        public BorderButton()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty LeftImagePathProperty = BindableProperty.Create(
            propertyName: nameof(LeftImagePath),
            returnType: typeof(string),
            declaringType: typeof(BorderButton));

        public string LeftImagePath
        {
            get => (string)GetValue(LeftImagePathProperty);
            set => SetValue(LeftImagePathProperty, value);
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            propertyName: nameof(Text),
            returnType: typeof(string),
            declaringType: typeof(BorderButton));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            declaringType: typeof(BorderButton),
            defaultValue: Color.White,
            defaultBindingMode: BindingMode.TwoWay);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(BorderButton),
            defaultValue: 12d,
            defaultBindingMode: BindingMode.TwoWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            declaringType: typeof(BorderButton),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.TwoWay);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly BindableProperty IsLeftImageVisibleProperty = BindableProperty.Create(
            propertyName: nameof(IsLeftImageVisible),
            returnType: typeof(bool),
            declaringType: typeof(BorderButton),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsLeftImageVisible
        {
            get => (bool)GetValue(IsLeftImageVisibleProperty);
            set => SetValue(IsLeftImageVisibleProperty, value);
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
            propertyName: nameof(Command),
            returnType: typeof(ICommand),
            declaringType: typeof(SearchBar),
            defaultBindingMode: BindingMode.TwoWay);

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        #endregion
    }
}
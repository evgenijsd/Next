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

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
        propertyName: nameof(FontFamily),
        returnType: typeof(string),
        declaringType: typeof(InputButton),
        defaultValue: string.Empty,
        defaultBindingMode: BindingMode.TwoWay);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(InputButton),
            defaultValue: 12d,
            defaultBindingMode: BindingMode.TwoWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty HeightRequestLeftImageProperty = BindableProperty.Create(
            propertyName: nameof(HeightRequestLeftImageProperty),
            returnType: typeof(double),
            declaringType: typeof(InputButton),
            defaultValue: 12d,
            defaultBindingMode: BindingMode.TwoWay);

        public double HeightRequestLeftImage
        {
            get => (double)GetValue(HeightRequestLeftImageProperty);
            set => SetValue(HeightRequestLeftImageProperty, value);
        }

        public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(
            propertyName: nameof(VerticalTextAlignment),
            returnType: typeof(TextAlignment),
            declaringType: typeof(InputButton),
            defaultBindingMode: BindingMode.TwoWay);

        public TextAlignment VerticalTextAlignment
        {
            get => (TextAlignment)GetValue(VerticalTextAlignmentProperty);
            set => SetValue(VerticalTextAlignmentProperty, value);
        }

        public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(
            propertyName: nameof(HorizontalTextAlignment),
            returnType: typeof(TextAlignment),
            declaringType: typeof(InputButton),
            defaultBindingMode: BindingMode.TwoWay);

        public TextAlignment HorizontalTextAlignment
        {
            get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty);
            set => SetValue(HorizontalTextAlignmentProperty, value);
        }

        public static readonly BindableProperty MarginTextProperty = BindableProperty.Create(
            propertyName: nameof(MarginText),
            returnType: typeof(Thickness),
            declaringType: typeof(InputButton),
            defaultBindingMode: BindingMode.TwoWay);

        public Thickness MarginText
        {
            get => (Thickness)GetValue(MarginTextProperty);
            set => SetValue(MarginTextProperty, value);
        }

        public static readonly BindableProperty MarginLeftImageProperty = BindableProperty.Create(
            propertyName: nameof(MarginLeftImage),
            returnType: typeof(Thickness),
            declaringType: typeof(InputButton),
            defaultBindingMode: BindingMode.TwoWay);

        public Thickness MarginLeftImage
        {
            get => (Thickness)GetValue(MarginLeftImageProperty);
            set => SetValue(MarginLeftImageProperty, value);
        }

        public static readonly BindableProperty LeftImagePathProperty = BindableProperty.Create(
         propertyName: nameof(LeftImagePath),
         returnType: typeof(string),
         declaringType: typeof(InputButton),
         defaultBindingMode: BindingMode.TwoWay);

        public string LeftImagePath
        {
            get => (string)GetValue(LeftImagePathProperty);
            set => SetValue(LeftImagePathProperty, value);
        }

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

        public static readonly BindableProperty IsLeftImageVisibleProperty = BindableProperty.Create(
            propertyName: nameof(IsLeftImageVisibleProperty),
            returnType: typeof(bool),
            declaringType: typeof(InputButton),
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsLeftImageVisible
        {
            get => (bool)GetValue(IsLeftImageVisibleProperty);
            set => SetValue(IsLeftImageVisibleProperty, value);
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
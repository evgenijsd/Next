using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class CustomStepper : StackLayout
    {
        public CustomStepper()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty DecrementIconSourceProperty = BindableProperty.Create(
            propertyName: nameof(DecrementIconSource),
            returnType: typeof(string),
            defaultValue: "ic_minus_primary_32x32",
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public string DecrementIconSource
        {
            get => (string)GetValue(DecrementIconSourceProperty);
            set => SetValue(DecrementIconSourceProperty, value);
        }

        public static readonly BindableProperty IncrementIconSourceProperty = BindableProperty.Create(
            propertyName: nameof(IncrementIconSource),
            returnType: typeof(string),
            defaultValue: "ic_plus_primary_32x32",
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public string IncrementIconSource
        {
            get => (string)GetValue(IncrementIconSourceProperty);
            set => SetValue(IncrementIconSourceProperty, value);
        }

        public static readonly BindableProperty IconSizesProperty = BindableProperty.Create(
            propertyName: nameof(IconSizes),
            returnType: typeof(int),
            defaultValue: 30,
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public int IconSizes
        {
            get => (int)GetValue(IconSizesProperty);
            set => SetValue(IconSizesProperty, value);
        }

        public static readonly BindableProperty TextWidthProperty = BindableProperty.Create(
            propertyName: nameof(TextWidth),
            returnType: typeof(int),
            defaultValue: 22,
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public int TextWidth
        {
            get => (int)GetValue(TextWidthProperty);
            set => SetValue(TextWidthProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            propertyName: nameof(TextColor),
            returnType: typeof(Color),
            defaultValue: (Color)Application.Current.Resources["TextAndBackgroundColor_i1"],
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(
            propertyName: nameof(FontFamily),
            returnType: typeof(string),
            defaultValue: "Barlow-Bold",
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            defaultValue: (double)Application.Current.Resources["TSize_i6"],
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty ValueFormatProperty = BindableProperty.Create(
            propertyName: nameof(ValueFormat),
            returnType: typeof(string),
            defaultValue: "{0}",
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public string ValueFormat
        {
            get => (string)GetValue(ValueFormatProperty);
            set => SetValue(ValueFormatProperty, value);
        }

        public static readonly BindableProperty ValueProperty = BindableProperty.Create(
            propertyName: nameof(Value),
            returnType: typeof(int),
            defaultValue: 1,
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly BindableProperty IncrementValueProperty = BindableProperty.Create(
            propertyName: nameof(IncrementValue),
            returnType: typeof(int),
            defaultValue: 1,
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public int IncrementValue
        {
            get => (int)GetValue(IncrementValueProperty);
            set => SetValue(IncrementValueProperty, value);
        }

        public static readonly BindableProperty MinValueProperty = BindableProperty.Create(
            propertyName: nameof(MinValue),
            returnType: typeof(int),
            defaultValue: 1,
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly BindableProperty MaxValueProperty = BindableProperty.Create(
            propertyName: nameof(MaxValue),
            returnType: typeof(int),
            defaultValue: 10,
            declaringType: typeof(CustomStepper),
            defaultBindingMode: BindingMode.TwoWay);

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public bool CanDecrement { get; set; }

        public bool CanIncrement { get; set; }

        public string DisplayingValue { get; private set; }

        private ICommand _decrementCommand;
        public ICommand DecrementCommand => _decrementCommand ??= new Command(OnDecrementCommand);

        private ICommand _incrementCommand;
        public ICommand IncrementCommand => _incrementCommand ??= new Command(OnIncrementCommand);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName
                is nameof(Value)
                or nameof(IncrementValue)
                or nameof(MinValue)
                or nameof(MaxValue))
            {
                CanDecrement = (Value - IncrementValue) >= MinValue;
                CanIncrement = (Value + IncrementValue) <= MaxValue;
            }

            if (propertyName
                is nameof(Value)
                or nameof(ValueFormat))
            {
                try
                {
                    DisplayingValue = ValueFormat != null
                        ? string.Format(ValueFormat, Value)
                        : Value.ToString();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion

        #region -- Private helpers --

        private void OnDecrementCommand()
        {
            Value -= IncrementValue;
        }

        private void OnIncrementCommand()
        {
            Value += IncrementValue;
        }

        #endregion
    }
}
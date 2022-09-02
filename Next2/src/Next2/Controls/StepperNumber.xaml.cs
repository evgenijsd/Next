using Next2.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Next2.Controls
{
    public partial class StepperNumber : StackLayout
    {
        public StepperNumber()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(StepperNumber),
            defaultBindingMode: BindingMode.OneWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty NumberProperty = BindableProperty.Create(
            propertyName: nameof(Value),
            returnType: typeof(int),
            declaringType: typeof(StepperNumber),
            defaultBindingMode: BindingMode.TwoWay);

        public int Value
        {
            get => (int)GetValue(NumberProperty);
            set => SetValue(NumberProperty, value);
        }

        public static readonly BindableProperty MinNumberProperty = BindableProperty.Create(
            propertyName: nameof(MinNumber),
            returnType: typeof(int),
            defaultValue: 0,
            declaringType: typeof(StepperNumber),
            defaultBindingMode: BindingMode.OneWay);

        public int MinNumber
        {
            get => (int)GetValue(MinNumberProperty);
            set => SetValue(MinNumberProperty, value);
        }

        public static readonly BindableProperty MaxNumberProperty = BindableProperty.Create(
            propertyName: nameof(MaxNumber),
            returnType: typeof(int),
            defaultValue: 10,
            declaringType: typeof(StepperNumber),
            defaultBindingMode: BindingMode.OneWay);

        public int MaxNumber
        {
            get => (int)GetValue(MaxNumberProperty);
            set => SetValue(MaxNumberProperty, value);
        }

        public static readonly BindableProperty ImageSizeProperty = BindableProperty.Create(
            propertyName: nameof(ImageSize),
            returnType: typeof(double),
            declaringType: typeof(StepperNumber),
            defaultBindingMode: BindingMode.OneWay);

        public double ImageSize
        {
            get => (double)GetValue(ImageSizeProperty);
            set => SetValue(ImageSizeProperty, value);
        }

        public static readonly BindableProperty NumberChangedActionProperty = BindableProperty.Create(
            propertyName: nameof(NumberChangedAction),
            returnType: typeof(Action),
            declaringType: typeof(StepperNumber),
            defaultBindingMode: BindingMode.OneWay);

        public Action NumberChangedAction
        {
            get => (Action)GetValue(NumberChangedActionProperty);
            set => SetValue(NumberChangedActionProperty, value);
        }

        private ICommand? _changeNumberCommand;
        public ICommand ChangeNumberCommand => _changeNumberCommand ??= new AsyncCommand<ENumberChange?>(OnChangeNumberCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Private helpers --

        private Task OnChangeNumberCommandAsync(ENumberChange? numberChange)
        {
            int number = Value;

            switch (numberChange ?? ENumberChange.None)
            {
                case ENumberChange.Increment:
                    number++;
                    number = number > MaxNumber
                        ? MinNumber
                        : number;

                    break;
                case ENumberChange.Decrement:
                    number--;
                    number = number < MinNumber
                        ? MaxNumber
                        : number;

                    break;
            }

            Value = number;

            NumberChangedAction();

            return Task.CompletedTask;
        }

        #endregion
    }
}
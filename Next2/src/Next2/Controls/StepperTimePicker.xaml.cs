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
    public partial class StepperTimePicker : Grid
    {
        public StepperTimePicker()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public int Hour { get; set; }

        public int Minute { get; set; }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(StepperTimePicker),
            defaultBindingMode: BindingMode.OneWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty TimeProperty = BindableProperty.Create(
            propertyName: nameof(Time),
            returnType: typeof(DateTime),
            declaringType: typeof(StepperTimePicker),
            defaultBindingMode: BindingMode.TwoWay);

        public DateTime Time
        {
            get => (DateTime)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public static readonly BindableProperty StartTimeProperty = BindableProperty.Create(
            propertyName: nameof(StartTime),
            returnType: typeof(DateTime),
            declaringType: typeof(StepperTimePicker),
            defaultBindingMode: BindingMode.TwoWay);

        public DateTime StartTime
        {
            get => (DateTime)GetValue(StartTimeProperty);
            set => SetValue(StartTimeProperty, value);
        }

        public static readonly BindableProperty ImageSizeProperty = BindableProperty.Create(
            propertyName: nameof(ImageSize),
            returnType: typeof(double),
            declaringType: typeof(StepperTimePicker),
            defaultBindingMode: BindingMode.OneWay);

        public double ImageSize
        {
            get => (double)GetValue(ImageSizeProperty);
            set => SetValue(ImageSizeProperty, value);
        }

        private ICommand? _changeTimeHoldCommand;
        public ICommand ChangeTimeHoldCommand => _changeTimeHoldCommand ??= new AsyncCommand<EHoldChange?>(OnChangeTimeHoldCommandAsync, allowsMultipleExecutions: false);

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(StartTime))
            {
                if (Time < StartTime)
                {
                    Time = StartTime;
                }

                Hour = Time.Hour;
                Minute = Time.Minute;
            }
        }

        #endregion

        #region -- Private helpers --

        private Task OnChangeTimeHoldCommandAsync(EHoldChange? holdChange)
        {
            var hour = Hour;
            var minute = Minute;

            switch (holdChange ?? EHoldChange.None)
            {
                case EHoldChange.HourIncrement:
                    hour++;
                    hour = hour > 23
                        ? 0
                        : hour;

                    break;
                case EHoldChange.HourDecrement:
                    hour--;
                    hour = hour < 0
                        ? 23
                        : hour;

                    break;
                case EHoldChange.MinuteIncrement:
                    minute++;
                    minute = minute > 59
                        ? 0
                        : minute;

                    break;
                case EHoldChange.MinuteDecrement:
                    minute--;
                    minute = minute < 0
                        ? 59
                        : minute;

                    break;
            }

            var time = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, hour, minute, second: 0);

            if (time >= StartTime)
            {
                Hour = hour;
                Minute = minute;
                Time = time;
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
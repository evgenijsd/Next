using Next2.Enums;
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
    public partial class HoldClock : Grid
    {
        public HoldClock()
        {
            InitializeComponent();

            Device.StartTimer(TimeSpan.FromSeconds(Constants.Limits.HELD_DISH_RELEASE_FREQUENCY), OnTimerTick);
        }

        #region -- Public properties --

        public int Hour { get; set; }

        public int Minute { get; set; }

        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
            propertyName: nameof(FontSize),
            returnType: typeof(double),
            declaringType: typeof(HoldClock),
            defaultBindingMode: BindingMode.OneWay);

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public static readonly BindableProperty HoldTimeProperty = BindableProperty.Create(
            propertyName: nameof(HoldTime),
            returnType: typeof(DateTime),
            declaringType: typeof(HoldClock),
            defaultBindingMode: BindingMode.TwoWay);

        public DateTime HoldTime
        {
            get => (DateTime)GetValue(HoldTimeProperty);
            set => SetValue(HoldTimeProperty, value);
        }

        public static readonly BindableProperty CurrentTimeProperty = BindableProperty.Create(
            propertyName: nameof(CurrentTime),
            returnType: typeof(DateTime),
            declaringType: typeof(HoldClock),
            defaultBindingMode: BindingMode.TwoWay);

        public DateTime CurrentTime
        {
            get => (DateTime)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }

        public static readonly BindableProperty ImageHeightProperty = BindableProperty.Create(
            propertyName: nameof(ImageHeight),
            returnType: typeof(double),
            declaringType: typeof(HoldClock),
            defaultBindingMode: BindingMode.OneWay);

        public double ImageHeight
        {
            get => (double)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }

        public static readonly BindableProperty ImageWidthProperty = BindableProperty.Create(
            propertyName: nameof(ImageWidth),
            returnType: typeof(double),
            declaringType: typeof(HoldClock),
            defaultBindingMode: BindingMode.OneWay);

        public double ImageWidth
        {
            get => (double)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        private ICommand? _changeTimeHoldCommand;
        public ICommand ChangeTimeHoldCommand => _changeTimeHoldCommand ??= new AsyncCommand<EHoldChange?>(OnChangeTimeHoldCommandAsync, allowsMultipleExecutions: false);

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

            var holdTime = new DateTime(CurrentTime.Year, CurrentTime.Month, CurrentTime.Day, hour, minute, second: 0);

            if (holdTime >= CurrentTime)
            {
                Hour = hour;
                Minute = minute;
                HoldTime = holdTime;
            }

            return Task.CompletedTask;
        }

        private bool OnTimerTick()
        {
            CurrentTime = DateTime.Now;

            if (HoldTime < CurrentTime)
            {
                HoldTime = CurrentTime;
            }

            Hour = HoldTime.Hour;
            Minute = HoldTime.Minute;

            return true;
        }

        #endregion
    }
}
using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class TimeAndDateBar : StackLayout
    {
        public TimeAndDateBar()
        {
            InitializeComponent();
            Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
        }

        #region -- Public properties --

        public static readonly BindableProperty IsRunningProperty = BindableProperty.Create(
            propertyName: nameof(IsRunning),
            returnType: typeof(bool),
            defaultValue: true,
            declaringType: typeof(TimeAndDateBar),
            defaultBindingMode: BindingMode.OneWay);

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public DateTime DateTime { get; private set; }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(IsRunning) && IsRunning)
            {
                DateTime = DateTime.Now;
                Device.StartTimer(TimeSpan.FromSeconds(1), OnTimerTick);
            }
        }

        #endregion

        #region -- Private helpers --

        private bool OnTimerTick()
        {
            DateTime = DateTime.Now;
            return IsRunning;
        }

        #endregion
    }
}
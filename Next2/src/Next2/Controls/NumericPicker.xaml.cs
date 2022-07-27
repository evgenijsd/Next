using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class NumericPicker : CarouselView
    {
        public NumericPicker()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty MinimumValueProperty = BindableProperty.Create(
            propertyName: nameof(MinimumValue),
            returnType: typeof(int),
            declaringType: typeof(NumericPicker));

        public int MinimumValue
        {
            get => (int)GetValue(MinimumValueProperty);
            set => SetValue(MinimumValueProperty, value);
        }

        public static readonly BindableProperty MaximumValueProperty = BindableProperty.Create(
            propertyName: nameof(MaximumValue),
            returnType: typeof(int),
            declaringType: typeof(NumericPicker));

        public int MaximumValue
        {
            get => (int)GetValue(MaximumValueProperty);
            set => SetValue(MaximumValueProperty, value);
        }

        public static readonly BindableProperty StringFormatProperty = BindableProperty.Create(
            propertyName: nameof(StringFormat),
            returnType: typeof(string),
            declaringType: typeof(NumericPicker));

        public string StringFormat
        {
            get => (string)GetValue(StringFormatProperty);
            set => SetValue(StringFormatProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(MinimumValue) or nameof(MaximumValue))
            {
                TryInitData(MinimumValue, MaximumValue);
                var dds = carouselView;
            }
        }

        #endregion

        #region -- Private helpers --

        private void TryInitData(int min, int max)
        {
            try
            {
                var list = new List<string>();

                for (int i = min; i <= max; i++)
                {
                    list.Add(i.ToString(StringFormat));
                }

                ItemsSource = list;
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
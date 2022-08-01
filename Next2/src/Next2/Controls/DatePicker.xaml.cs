using System;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class DatePicker : Frame
    {
        public DatePicker()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty DateLabelProperty = BindableProperty.Create(
            propertyName: nameof(DateLabel),
            returnType: typeof(DateTime),
            defaultBindingMode: BindingMode.TwoWay,
            defaultValue: DateTime.Now,
            declaringType: typeof(DatePicker));

        public DateTime DateLabel
        {
            get => (DateTime)GetValue(DateLabelProperty);
            set => SetValue(DateLabelProperty, value);
        }

        #endregion
    }
}
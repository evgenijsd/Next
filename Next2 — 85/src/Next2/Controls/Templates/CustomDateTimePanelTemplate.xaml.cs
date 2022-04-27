using Next2.Views;
using System;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class CustomDateTimePanelTemplate : ContentView
    {
        public CustomDateTimePanelTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty CurrentDateTimeProperty = BindableProperty.Create(
            propertyName: nameof(CurrentDateTime),
            returnType: typeof(DateTime),
            declaringType: typeof(CustomDateTimePanelTemplate),
            defaultBindingMode: BindingMode.TwoWay);

        public DateTime CurrentDateTime
        {
            get => (DateTime)GetValue(CurrentDateTimeProperty);
            set => SetValue(CurrentDateTimeProperty, value);
        }

        #endregion

    }
}
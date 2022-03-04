using Next2.Models;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Next2.Controls.Templates
{
    public partial class SeatItemTemplate : StackLayout
    {
        public SeatItemTemplate()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty HeightListProperty = BindableProperty.Create(
            propertyName: nameof(HeightList),
            returnType: typeof(double),
            declaringType: typeof(SeatItemTemplate));

        public double HeightList
        {
            get => (double)GetValue(HeightListProperty);
            set => SetValue(HeightListProperty, value);
        }

        #endregion

        #region --Overrides--

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (BindingContext is SeatBindableModel context)
            {
                HeightList = context.Sets.Count * 95;
            }
        }

        #endregion
    }
}
using Next2.Behaviors;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class RotatingGears : Grid
    {
        public RotatingGears()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IsAnimateProperty = BindableProperty.Create(
            propertyName: nameof(IsAnimate),
            returnType: typeof(bool),
            declaringType: typeof(RotatingGears),
            defaultValue: false,
            defaultBindingMode: BindingMode.OneWay);

        public bool IsAnimate
        {
            get => (bool)GetValue(IsAnimateProperty);
            set => SetValue(IsAnimateProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsAnimate))
            {
                if (IsAnimate)
                {
                    grid.Behaviors.Add(new RotationBehavior() { SpeedRpm = 4 });
                    gearB.Behaviors.Add(new RotationBehavior() { SpeedRpm = -30 });
                    gearL.Behaviors.Add(new RotationBehavior() { SpeedRpm = -49 });
                    gearU.Behaviors.Add(new RotationBehavior() { SpeedRpm = 60 });
                }
                else
                {
                    grid.Behaviors.Clear();
                    gearB.Behaviors.Clear();
                    gearL.Behaviors.Clear();
                    gearU.Behaviors.Clear();
                }
            }
        }

        #endregion
    }
}
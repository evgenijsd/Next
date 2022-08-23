using Xamarin.Forms;

namespace Next2.Behaviors
{
    internal class RotationBehavior : Behavior<VisualElement>
    {
        private VisualElement? _visualElement;
        private Animation? _animation;

        #region -- Public properties --

        public static readonly BindableProperty IsAnimateProperty = BindableProperty.Create(
            propertyName: nameof(IsAnimate),
            returnType: typeof(bool),
            declaringType: typeof(RotationBehavior),
            propertyChanged: OnIsAnimatePropertyChanged,
            defaultValue: true,
            defaultBindingMode: BindingMode.OneWay);

        public bool IsAnimate
        {
            get => (bool)GetValue(IsAnimateProperty);
            set => SetValue(IsAnimateProperty, value);
        }

        public static readonly BindableProperty SpeedRpmProperty = BindableProperty.Create(
            propertyName: nameof(SpeedRpm),
            returnType: typeof(double),
            declaringType: typeof(RotationBehavior),
            propertyChanged: OnSpeedRpmPropertyChanged,
            defaultValue: 30d,
            defaultBindingMode: BindingMode.OneWay);

        public double SpeedRpm
        {
            get => (double)GetValue(SpeedRpmProperty);
            set => SetValue(SpeedRpmProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnAttachedTo(VisualElement bindable)
        {
            base.OnAttachedTo(bindable);

            _visualElement = bindable;

            Rotate();
        }

        protected override void OnDetachingFrom(VisualElement bindable)
        {
            bindable.AbortAnimation("RotateAnimation");

            _visualElement = null;

            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region -- Private helpers --

        private static void OnIsAnimatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
            => ((RotationBehavior)bindable).Rotate();

        private static void OnSpeedRpmPropertyChanged(BindableObject bindable, object oldValue, object newValue)
            => ((RotationBehavior)bindable).Rotate();

        private void Rotate()
        {
            if (_visualElement == null)
            {
                return;
            }

            _visualElement.AbortAnimation("RotateAnimation");

            if (IsAnimate)
            {
                _animation = new Animation(v => _visualElement.Rotation = v, 0, 360 * SpeedRpm);

                _animation.Commit(_visualElement, "RotateAnimation", 16, 60000, Easing.Linear, (v, c) => _visualElement.Rotation = 0, () => true);
            }
            else
            {
                _visualElement.AbortAnimation("RotateAnimation");
            }
        }

        #endregion
    }
}

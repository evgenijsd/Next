using Next2.Controls.Buttons;
using System.ComponentModel;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class InputButtonBehavior : Behavior<InputButton>
    {
        private InputButton _inputButton;

        #region -- Overrides --

        protected override void OnAttachedTo(InputButton inputButton)
        {
            base.OnAttachedTo(inputButton);

            _inputButton = inputButton;
            _inputButton.PropertyChanged += InputButtonPropertyChanged;
        }

        protected override void OnDetachingFrom(InputButton inputButton)
        {
            base.OnDetachingFrom(inputButton);

            _inputButton.PropertyChanged -= InputButtonPropertyChanged;
            _inputButton = null;
        }

        #endregion

        #region -- Private helpers --

        private void InputButtonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == InputButton.TextProperty.PropertyName || e.PropertyName == InputButton.IsValidValueProperty.PropertyName)
                && _inputButton.Text != string.Empty)
            {
                if (_inputButton.IsValidValue)
                {
                    _inputButton.SetDynamicResource(InputButton.BorderColorProperty, "TextAndBackgroundColor_i2");
                    _inputButton.IsLeftImageVisible = false;
                    _inputButton.SetDynamicResource(InputButton.TextColorProperty, "TextAndBackgroundColor_i1");
                }
                else
                {
                    _inputButton.SetDynamicResource(InputButton.BorderColorProperty, "IndicationColor_i3");
                    _inputButton.IsLeftImageVisible = true;
                    _inputButton.SetDynamicResource(InputButton.TextColorProperty, "TextAndBackgroundColor_i1");
                }
            }
            else if (_inputButton.Text == string.Empty)
            {
                _inputButton.SetDynamicResource(InputButton.BorderColorProperty, "TextAndBackgroundColor_i2");
                _inputButton.IsLeftImageVisible = false;
                _inputButton.SetDynamicResource(InputButton.TextColorProperty, "TextAndBackgroundColor_i9");
            }
        }

        #endregion

    }
}

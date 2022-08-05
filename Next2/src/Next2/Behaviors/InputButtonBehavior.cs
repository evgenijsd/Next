using Next2.Controls.Buttons;
using System.ComponentModel;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class InputButtonBehavior : Behavior<InputButton>
    {
        #region -- Overrides --

        protected override void OnAttachedTo(InputButton inputButton)
        {
            base.OnAttachedTo(inputButton);

            inputButton.PropertyChanged += InputButtonPropertyChanged;
        }

        protected override void OnDetachingFrom(InputButton inputButton)
        {
            inputButton.PropertyChanged -= InputButtonPropertyChanged;

            base.OnDetachingFrom(inputButton);
        }

        #endregion

        #region -- Private helpers --

        private void InputButtonPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(InputButton.Text) or nameof(InputButton.IsValidValue)
                && sender is InputButton inputButton)
            {
                if (string.IsNullOrEmpty(inputButton.Text))
                {
                    inputButton.SetDynamicResource(InputButton.BorderColorProperty, "TextAndBackgroundColor_i2");
                    inputButton.IsLeftImageVisible = false;
                    inputButton.SetDynamicResource(InputButton.TextColorProperty, "TextAndBackgroundColor_i9");
                }
                else
                {
                    if (inputButton.IsValidValue)
                    {
                        inputButton.SetDynamicResource(InputButton.BorderColorProperty, "TextAndBackgroundColor_i2");
                        inputButton.IsLeftImageVisible = false;
                        inputButton.SetDynamicResource(InputButton.TextColorProperty, "TextAndBackgroundColor_i1");
                    }
                    else
                    {
                        inputButton.SetDynamicResource(InputButton.BorderColorProperty, "IndicationColor_i3");
                        inputButton.IsLeftImageVisible = true;
                        inputButton.SetDynamicResource(InputButton.TextColorProperty, "TextAndBackgroundColor_i1");
                    }
                }
            }
        }

        #endregion
    }
}

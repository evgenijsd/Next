using Next2.Controls.Buttons;
using Next2.Resources.Strings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Resources;
using System.Text;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Next2.Behaviors
{
    public class InputButtonBehavior : Behavior<InputButton>
    {
        private InputButton _inputButton;

        private string title = LocalizationResourceManager.Current["TypeEmployeeId"];

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
            if (e.PropertyName == InputButton.TextProperty.PropertyName && _inputButton.Text != title)
            {
                if (_inputButton.IsEmployeeExists == true)
                {
                    _inputButton.SetDynamicResource(InputButton.BorderColorProperty, "TextAndBackgroundColor_i2");
                    _inputButton.IsErrorNotificationVisible = false;
                    _inputButton.SetDynamicResource(InputButton.TextColorProperty, "TextAndBackgroundColor_i1");
                }
                else
                {
                    _inputButton.SetDynamicResource(InputButton.BorderColorProperty, "IndicationColor_i3");
                    _inputButton.IsErrorNotificationVisible = true;
                    _inputButton.SetDynamicResource(InputButton.TextColorProperty, "TextAndBackgroundColor_i1");
                }
            }
            else if (_inputButton.Text == title)
            {
                _inputButton.SetDynamicResource(InputButton.BorderColorProperty, "TextAndBackgroundColor_i2");
                _inputButton.IsErrorNotificationVisible = false;
                _inputButton.SetDynamicResource(InputButton.TextColorProperty, "TextAndBackgroundColor_i9");
            }
        }

        #endregion

    }
}

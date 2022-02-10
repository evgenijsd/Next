using Next2.Controls.Buttons;
using Next2.Resources.Strings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Next2.Behaviors
{
    public class BorderButtonBehavior : Behavior<BorderButton>
    {
        private BorderButton _button;
        private string _title = LocalizationResourceManager.Current["TypeEmployeeId"];

        public static readonly BindableProperty TitleProperty = BindableProperty.Create(
            propertyName: nameof(TitleProperty),
            returnType: typeof(string),
            declaringType: typeof(BorderButtonBehavior),
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (b, o, n) => ((BorderButtonBehavior)b).titlepropertyChanged((object)o, (object)n));

        private void titlepropertyChanged(object oldValue, object newValue)
        {
            if (Title != _title)
            {
                _button.SetDynamicResource(BorderButton.BorderColorProperty, "IndicationColor_i3");
                _button.SetDynamicResource(BorderButton.TextColorProperty, "TextAndBackgroundColor_i1");
                _button.IsEnabled = true;
            }
            else if (Title == _title)
            {
                _button.SetDynamicResource(BorderButton.BorderColorProperty, "IndicationColor_i4");
                _button.SetDynamicResource(BorderButton.TextColorProperty, "TextAndBackgroundColor_i7");
                _button.IsEnabled = false;
            }
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        protected override void OnAttachedTo(BorderButton button)
        {
            base.OnAttachedTo(button);

            _button = button;
        }

        protected override void OnDetachingFrom(BorderButton button)
        {
            base.OnDetachingFrom(button);
            _button = null;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == TitleProperty.PropertyName)
            {
                Console.WriteLine();
            }
        }
    }
}

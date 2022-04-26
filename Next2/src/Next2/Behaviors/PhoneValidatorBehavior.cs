using Next2.Controls;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class PhoneValidatorBehavior : Behavior<NoActionMenuEntry>
    {
        #region -- Overrides --

        protected override void OnAttachedTo(NoActionMenuEntry bindable)
        {
            bindable.TextChanged += OnTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(NoActionMenuEntry bindable)
        {
            bindable.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region-- Private helpers --

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is NoActionMenuEntry entry && e.NewTextValue is not null)
            {
                entry.IsValid = Regex.IsMatch(e.NewTextValue, Constants.Validators.PHONE, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
        }

        #endregion
    }
}

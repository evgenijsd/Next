using Next2.Controls;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class EmailValidatorBehavior : Behavior<CustomEntry>
    {
        #region -- Overrides --

        protected override void OnAttachedTo(CustomEntry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(CustomEntry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region -- Private Helpers --

        private void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            bool isValid = false;
            if (e.NewTextValue != null)
            {
                isValid = Regex.IsMatch(e.NewTextValue, Constants.EMAIL_REGEX, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                var entry = sender as CustomEntry;
                if (entry is not null)
                {
                    entry.IsValid = isValid;
                }
            }
        }

        #endregion
    }
}

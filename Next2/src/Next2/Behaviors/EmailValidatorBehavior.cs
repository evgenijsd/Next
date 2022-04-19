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

        #region -- Private helpers --

        private void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is CustomEntry entry && e.NewTextValue is not null)
            {
                entry.IsValid = Regex.IsMatch(e.NewTextValue, Constants.Validators.EMAIL, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
        }

        #endregion
    }
}

using Next2.Controls;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class EmailValidatorBehavior : Behavior<NoActionMenuEntry>
    {
        #region -- Overrides --

        protected override void OnAttachedTo(NoActionMenuEntry bindable)
        {
            bindable.TextChanged += HandleTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(NoActionMenuEntry bindable)
        {
            bindable.TextChanged -= HandleTextChanged;
            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region -- Private helpers --

        private void HandleTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is NoActionMenuEntry entry && e.NewTextValue is not null)
            {
                entry.IsValid = Regex.IsMatch(e.NewTextValue, Constants.Validators.EMAIL, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
        }

        #endregion
    }
}

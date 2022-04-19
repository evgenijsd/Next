using Next2.Controls;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class PhoneValidatorBehavior : Behavior<HideClipboardEntry>
    {
        #region -- Overrides --

        protected override void OnAttachedTo(HideClipboardEntry bindable)
        {
            bindable.TextChanged += OnTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(HideClipboardEntry bindable)
        {
            bindable.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region-- Private helpers --

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is HideClipboardEntry entry && e.NewTextValue is not null)
            {
                entry.IsValid = Regex.IsMatch(e.NewTextValue, Constants.Validators.PHONE, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
        }

        private string ToPascalCase(string word)
        {
            return word[0].ToString().ToUpper() + word[1..].ToLower();
        }

        #endregion
    }
}

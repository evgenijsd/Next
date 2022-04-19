using Next2.Controls;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class NameValidatorBehavior : Behavior<HideClipboardEntry>
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
            if (sender is HideClipboardEntry entry && e.NewTextValue is not null && e.NewTextValue.Any())
            {
                bool isMatch = false;

                try
                {
                    isMatch = Regex.IsMatch(e.NewTextValue, Constants.Validators.CUSTOMER_NAME, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100));
                }
                catch (Exception)
                {
                }

                if (!isMatch)
                {
                    entry.Text = e.OldTextValue;
                }
                else if (e.NewTextValue.Last() != ' ')
                {
                    var words = e.NewTextValue
                       .Split(' ')
                       .Where(x => !string.IsNullOrWhiteSpace(x))
                       .Select(x => WordToPascalCase(x));

                    entry.Text = string.Join(' ', words);
                }
            }
        }

        private string WordToPascalCase(string word)
        {
            return word[0].ToString().ToUpper() + word[1..].ToLower();
        }

        #endregion
    }
}

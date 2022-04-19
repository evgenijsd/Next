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
                int cursorPosition = entry.CursorPosition;

                try
                {
                    if (Regex.IsMatch(e.NewTextValue, Constants.Validators.CUSTOMER_NAME, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100)))
                    {
                        string formattedText = e.NewTextValue;

                        foreach (var matchesWord in Regex.Matches(formattedText, @"[a-z]+", RegexOptions.IgnoreCase))
                        {
                            string word = $"{matchesWord}";

                            if (!Regex.IsMatch(word, Constants.Validators.PASCAL_CASE))
                            {
                                string newWord = ToPascalCase(word);

                                formattedText = Regex.Replace(formattedText, word, newWord);
                            }
                        }

                        entry.Text = formattedText;
                    }
                    else
                    {
                        entry.Text = e.OldTextValue;
                    }
                }
                catch (Exception ex)
                {
                    entry.Text = e.OldTextValue;
                }

                entry.CursorPosition = cursorPosition;
            }
        }

        private string ToPascalCase(string word)
        {
            return word[0].ToString().ToUpper() + word[1..].ToLower();
        }

        #endregion
    }
}

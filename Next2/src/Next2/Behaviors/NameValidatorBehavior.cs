﻿using Next2.Controls;
using System;
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

        #region -- Private helpers --

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is HideClipboardEntry entry && e.NewTextValue is not null)
            {
                try
                {
                    entry.IsValid = Regex.IsMatch(e.NewTextValue, Constants.Validators.CUSTOMER_NAME, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200));
                }
                catch (Exception)
                {
                    entry.IsValid = false;
                }

                string formattedText = e.NewTextValue;
                var words = Regex.Matches(formattedText, Constants.Validators.WORD, RegexOptions.IgnoreCase);

                foreach (var word in words)
                {
                    string wordToReplace = $"{word}";

                    if (!Regex.IsMatch(wordToReplace, Constants.Validators.PASCAL_CASE))
                    {
                        string pascalWord = ToPascalCase(wordToReplace);
                        formattedText = Regex.Replace(formattedText, wordToReplace, pascalWord);
                    }
                }

                entry.Text = formattedText;
            }
        }

        private string ToPascalCase(string word)
        {
            return word[0].ToString().ToUpper() + word[1..].ToLower();
        }

        #endregion
    }
}

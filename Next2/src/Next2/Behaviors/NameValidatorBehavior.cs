using Next2.Controls;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class NameValidatorBehavior : Behavior<NoActionMenuEntry>
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

        #region -- Private helpers --

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is NoActionMenuEntry entry)
            {
                if (!string.IsNullOrEmpty(e.NewTextValue))
                {
                    bool isNameCorrect = false;

                    try
                    {
                        isNameCorrect = Regex.IsMatch(e.NewTextValue, Constants.Validators.CUSTOMER_NAME, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(70));
                    }
                    catch (Exception)
                    {
                    }

                    if (!isNameCorrect)
                    {
                        entry.Text = e.OldTextValue;
                    }
                    else
                    {
                        var wordsWithIncorrectRegister = Regex.Matches(e.NewTextValue, Constants.Validators.WORD, RegexOptions.IgnoreCase)
                            .Where(x => !Regex.IsMatch(x.ToString(), Constants.Validators.PASCAL_CASE));

                        if (wordsWithIncorrectRegister.Any())
                        {
                            string nameWithCorrectedCase = e.NewTextValue;

                            foreach (var word in wordsWithIncorrectRegister)
                            {
                                string wordToReplace = $"{word}";
                                nameWithCorrectedCase = Regex.Replace(nameWithCorrectedCase, wordToReplace, ToPascalCase(wordToReplace));
                            }

                            entry.Text = nameWithCorrectedCase;
                        }
                    }
                }

                entry.IsValid = !string.IsNullOrEmpty(entry.Text);
            }
        }

        private string ToPascalCase(string word)
        {
            return word[0].ToString().ToUpper() + word[1..].ToLower();
        }

        #endregion
    }
}

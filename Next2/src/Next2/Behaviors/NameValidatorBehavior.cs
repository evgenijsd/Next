using Next2.Controls;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class NameValidatorBehavior : Behavior<CustomEntry>
    {
        #region -- Overrides --

        protected override void OnAttachedTo(CustomEntry bindable)
        {
            bindable.TextChanged += OnTextChanged;
            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(CustomEntry bindable)
        {
            bindable.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region-- Private helpers --

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is CustomEntry entry && e.NewTextValue is not null && e.NewTextValue.Any())
            {
                // 1) парсит норм, виснет
                //bool isMatch = Regex.IsMatch(e.NewTextValue, @"^([A-Za-z-,.']+\d*[ ]?)+$");

                // 2) парсит норм, виснет
                //bool isMatch = Regex.IsMatch(e.NewTextValue, @"^\b([A-Za-z-,.']+\d*[ ]*)+$");
                _ = Task.Delay(150);
                bool isMatch = Regex.IsMatch(e.NewTextValue, @"^\b([A-Za-z-,.']+\d*[ ]*)+$");

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

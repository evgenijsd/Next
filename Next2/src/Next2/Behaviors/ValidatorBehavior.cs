using Next2.Controls;
using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class ValidatorBehavior : Behavior<NoActionMenuEntry>
    {
        #region -- Public properties --

        public bool ShouldSetOldValidValue { get; set; }

        public string? MatchPattern { get; set; }

        public string? MismatchPattern { get; set; }

        #endregion

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
            if (sender is NoActionMenuEntry entry)
            {
                if (string.IsNullOrEmpty(e.NewTextValue))
                {
                    entry.IsValid = false;
                }
                else
                {
                    try
                    {
                        var isValid = true;

                        if (MatchPattern is not null)
                        {
                            isValid = Regex.IsMatch(e.NewTextValue, MatchPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                        }

                        if (MismatchPattern is not null)
                        {
                            isValid = !Regex.IsMatch(e.NewTextValue, MismatchPattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                        }

                        entry.IsValid = isValid;

                        if (ShouldSetOldValidValue && !isValid)
                        {
                            entry.Text = e.OldTextValue;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        #endregion
    }
}

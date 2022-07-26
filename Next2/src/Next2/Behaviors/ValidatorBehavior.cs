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

        public string? Match { get; set; }

        public string? NotMatch { get; set; }

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
                        var result = true;

                        if (Match is not null)
                        {
                            result = Regex.IsMatch(e.NewTextValue, Match, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                        }

                        if (NotMatch is not null)
                        {
                            result = !Regex.IsMatch(e.NewTextValue, NotMatch, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                        }

                        entry.IsValid = result;

                        if (ShouldSetOldValidValue && !result)
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

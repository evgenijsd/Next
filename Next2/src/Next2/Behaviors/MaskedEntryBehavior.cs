using Next2.Controls;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class MaskedEntryBehavior : Behavior<NoActionMenuEntry>
    {
        private IDictionary<int, char>? _positionsInsert;

        #region -- Public properties --

        public char UnMaskedCharacter { get; set; } = 'X';

        private string _mask = string.Empty;
        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;

                InitPositionsInsert();
            }
        }

        #endregion

        #region -- Overrides --

        protected override void OnAttachedTo(NoActionMenuEntry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(NoActionMenuEntry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        #endregion

        #region -- Private helpers --

        private void InitPositionsInsert()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positionsInsert = null;
            }
            else
            {
                var list = new Dictionary<int, char>();

                for (var i = 0; i < Mask.Length; i++)
                {
                    if (Mask[i] != UnMaskedCharacter)
                    {
                        list.Add(i, Mask[i]);
                    }
                }

                _positionsInsert = list;
            }
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            try
            {
                if (sender is NoActionMenuEntry entry)
                {
                    string text = new(entry.Text);

                    if (!string.IsNullOrEmpty(text) || _positionsInsert is not null)
                    {
                        if (text.Length > _mask.Length)
                        {
                            entry.Text = text.Remove(text.Length - 1);
                        }
                        else
                        {
                            foreach (var position in _positionsInsert)
                            {
                                if (text.Length >= position.Key + 1)
                                {
                                    var value = position.Value.ToString();

                                    if (text.Substring(position.Key, 1) != value)
                                    {
                                        text = text.Insert(position.Key, value);
                                    }
                                }
                            }

                            if (entry.Text != text)
                            {
                                entry.Text = text;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}

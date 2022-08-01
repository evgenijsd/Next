using Next2.Controls;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Next2.Behaviors
{
    public class MaskedEntryBehavior : Behavior<NoActionMenuEntry>
    {
        private IDictionary<int, char>? _positionsToInsert;

        #region -- Public properties --

        public char UnmaskedSymbol { get; set; } = 'X';

        private string _mask = string.Empty;
        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;

                InitPositionsForInsertion();
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

        private void InitPositionsForInsertion()
        {
            if (string.IsNullOrEmpty(Mask))
            {
                _positionsToInsert = null;
            }
            else
            {
                var positionsAndSymbolsPairs = new Dictionary<int, char>();

                for (var i = 0; i < Mask.Length; i++)
                {
                    if (Mask[i] != UnmaskedSymbol)
                    {
                        positionsAndSymbolsPairs.Add(i, Mask[i]);
                    }
                }

                _positionsToInsert = positionsAndSymbolsPairs;
            }
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            try
            {
                if (sender is NoActionMenuEntry entry && !string.IsNullOrEmpty(entry.Text))
                {
                    string text = new(entry.Text);

                    if (_positionsToInsert is not null)
                    {
                        if (text.Length > _mask.Length)
                        {
                            entry.Text = text.Remove(text.Length - 1);
                        }
                        else
                        {
                            foreach (var position in _positionsToInsert)
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

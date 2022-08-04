using Next2.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class MaskedBehavior : Behavior<VisualElement>
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

        protected override void OnAttachedTo(VisualElement visualElement)
        {
            if (visualElement is NoActionMenuEntry entry)
            {
                entry.TextChanged += OnEntryTextChanged;
            }
            else if (visualElement is Label label)
            {
                label.PropertyChanged += OnLabelPropertyChanged;
            }

            base.OnAttachedTo(visualElement);
        }

        protected override void OnDetachingFrom(VisualElement visualElement)
        {
            if (visualElement is NoActionMenuEntry entry)
            {
                entry.TextChanged -= OnEntryTextChanged;
            }
            else if (visualElement is Label label)
            {
                label.PropertyChanged -= OnLabelPropertyChanged;
            }

            base.OnDetachingFrom(visualElement);
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

        private void OnLabelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                if (sender is Label label && !string.IsNullOrEmpty(label.Text))
                {
                    var text = TryApplyMaskToText(label.Text);

                    if (label.Text != text)
                    {
                        label.Text = text;
                    }
                }
            }
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is NoActionMenuEntry entry && !string.IsNullOrEmpty(entry.Text))
            {
                var text = TryApplyMaskToText(entry.Text);

                if (entry.Text != text)
                {
                    entry.Text = text;
                }
            }
        }

        private string TryApplyMaskToText(string txt)
        {
            string result = txt;

            if (_positionsToInsert is not null)
            {
                string tempText = new(txt);

                try
                {
                    if (tempText.Length > _mask.Length)
                    {
                        result = tempText.Remove(tempText.Length - 1);
                    }
                    else
                    {
                        foreach (var position in _positionsToInsert)
                        {
                            if (tempText.Length >= position.Key + 1)
                            {
                                var value = position.Value.ToString();

                                if (tempText.Substring(position.Key, 1) != value)
                                {
                                    tempText = tempText.Insert(position.Key, value);
                                }
                            }
                        }

                        result = tempText;
                    }
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        #endregion
    }
}

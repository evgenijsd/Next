﻿using Next2.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class ScrollToCenterBehavior : Behavior<View>
    {
        private CustomCollectionView _collection = new();

        #region -- Overrides --

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);

            _collection = (CustomCollectionView)bindable;
            _collection.SelectionChanged += OnCollectionSelectionChanged;
        }

        protected override void OnDetachingFrom(View bindable)
        {
            _collection.SelectionChanged -= OnCollectionSelectionChanged;

            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region -- Private helpers --

        private void OnCollectionSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_collection is not null)
            {
                _collection.ScrollTo(_collection.SelectedItem, position: ScrollToPosition.Center, animate: false);
            }
        }

        #endregion
    }
}
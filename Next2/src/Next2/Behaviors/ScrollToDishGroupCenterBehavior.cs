using Next2.Controls;
using Next2.Models.Bindables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class ScrollToDishGroupCenterBehavior : Behavior<View>
    {
        private CollectionView _collection = new();

        #region -- Overrides --

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);

            if (bindable is CollectionView collection)
            {
                _collection = collection;
                _collection.SelectionChanged += OnCollectionSelectionChanged;
            }
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
            if (e.CurrentSelection.FirstOrDefault() is DishBindableModel dish)
            {
                _collection.ScrollTo(dish, dish, ScrollToPosition.Center, animate: false);
            }
        }

        #endregion
    }
}

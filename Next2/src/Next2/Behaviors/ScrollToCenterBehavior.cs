using Next2.Controls;
using System.Linq;
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

            if (bindable is CustomCollectionView collection)
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
            if (_collection is not null && _collection.SelectionMode != SelectionMode.None)
            {
                if (_collection.SelectionMode == SelectionMode.Single)
                {
                    _collection.ScrollTo(_collection.SelectedItem, position: ScrollToPosition.Center, animate: false);
                }
                else if (e.CurrentSelection.Count != e.PreviousSelection.Count)
                {
                    var selectedItem = e.CurrentSelection.Except(e.PreviousSelection).FirstOrDefault();
                    _collection.ScrollTo(selectedItem, position: ScrollToPosition.Center, animate: false);
                }
            }
        }

        #endregion
    }
}

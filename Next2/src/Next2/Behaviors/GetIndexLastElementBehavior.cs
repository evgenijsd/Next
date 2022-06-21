using Next2.Controls;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Next2.Behaviors
{
    public class GetIndexLastElementBehavior : Behavior<View>
    {
        private CustomCollectionView _collection = new();
        private double _viewItemHeight;

        #region -- Overrides --

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);

            _collection = (CustomCollectionView)bindable;
            _collection.Scrolled += OnCollectionScrolled;
        }

        protected override void OnDetachingFrom(View bindable)
        {
            _collection.Scrolled -= OnCollectionScrolled;

            base.OnDetachingFrom(bindable);
        }

        #endregion

        #region -- Private helpers --

        private void OnCollectionScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (_viewItemHeight == 0f && _collection.ItemTemplate.CreateContent() is View view)
            {
                _viewItemHeight = view.HeightRequest;
            }

            if (_viewItemHeight > 0)
            {
                _collection.IndexLastVisible = (int)((_collection.Height + e.VerticalOffset) / _viewItemHeight);
            }
        }

        #endregion
    }
}

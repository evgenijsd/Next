using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls
{
    public class ExpandedGridCollectionView : CollectionView
    {
        #region -- Public properties --

        public static readonly BindableProperty MinimumVisibleRowsProperty = BindableProperty.Create(
            propertyName: nameof(MinimumVisibleRows),
            returnType: typeof(int),
            declaringType: typeof(ExpandedGridCollectionView));

        public int MinimumVisibleRows
        {
            get => (int)GetValue(MinimumVisibleRowsProperty);
            set => SetValue(MinimumVisibleRowsProperty, value);
        }

        public static readonly BindableProperty MaximumVisibleRowsProperty = BindableProperty.Create(
            propertyName: nameof(MaximumVisibleRows),
            returnType: typeof(int),
            declaringType: typeof(ExpandedGridCollectionView));

        public int MaximumVisibleRows
        {
            get => (int)GetValue(MaximumVisibleRowsProperty);
            set => SetValue(MaximumVisibleRowsProperty, value);
        }

        public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(
            propertyName: nameof(IsExpanded),
            returnType: typeof(bool),
            declaringType: typeof(ExpandedGridCollectionView));

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName
                is nameof(IsExpanded)
                or nameof(MinimumVisibleRows)
                or nameof(this.ItemsLayout)
                or nameof(this.ItemTemplate))
            {
                CalculateHeightRequest();
            }
        }

        #endregion

        #region -- Private helpers --

        private void CalculateHeightRequest()
        {
            if (ItemsLayout is GridItemsLayout gridItemsLayout && ItemTemplate?.CreateContent() is View item)
            {
                var visibleRowsAmount = IsExpanded
                    ? Math.Ceiling((double)((ICollection)ItemsSource).Count / gridItemsLayout.Span)
                    : MinimumVisibleRows;

                HeightRequest = MinimumHeightRequest = visibleRowsAmount * (item.HeightRequest + gridItemsLayout.VerticalItemSpacing);
            }
        }

        #endregion
    }
}

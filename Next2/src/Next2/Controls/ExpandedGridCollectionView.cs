using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls
{
    public class ExpandedGridCollectionView : CollectionView
    {
        #region -- Public properties --

        public static readonly BindableProperty MinimumNumberVisibleRowsProperty = BindableProperty.Create(
            propertyName: nameof(MinimumNumberVisibleRows),
            returnType: typeof(int),
            declaringType: typeof(ExpandedGridCollectionView));

        public int MinimumNumberVisibleRows
        {
            get => (int)GetValue(MinimumNumberVisibleRowsProperty);
            set => SetValue(MinimumNumberVisibleRowsProperty, value);
        }

        public static readonly BindableProperty MaximumNumberVisibleRowsProperty = BindableProperty.Create(
            propertyName: nameof(MaximumNumberVisibleRows),
            returnType: typeof(int),
            declaringType: typeof(ExpandedGridCollectionView));

        public int MaximumNumberVisibleRows
        {
            get => (int)GetValue(MaximumNumberVisibleRowsProperty);
            set => SetValue(MaximumNumberVisibleRowsProperty, value);
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
                or nameof(MinimumNumberVisibleRows)
                or nameof(this.ItemsSource)
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
            if (ItemsLayout is GridItemsLayout gridItemsLayout && ItemTemplate?.CreateContent() is View itemTemplate)
            {
                if (ItemsSource is not ICollection collection || collection.Count == 0)
                {
                    HeightRequest = MinimumHeightRequest = 0;
                }
                else
                {
                    int numberVisibleRows = MinimumNumberVisibleRows;

                    if (IsExpanded)
                    {
                        int numberVisibleRowsIncludingColumns = (int)Math.Ceiling((double)collection.Count / gridItemsLayout.Span);

                        numberVisibleRows = numberVisibleRowsIncludingColumns < MaximumNumberVisibleRows
                            ? numberVisibleRowsIncludingColumns
                            : MaximumNumberVisibleRows;
                    }

                    HeightRequest = MinimumHeightRequest = numberVisibleRows * (itemTemplate.HeightRequest + gridItemsLayout.VerticalItemSpacing);
                }
            }
        }

        #endregion
    }
}
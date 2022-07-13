using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls
{
    public class ExpandedGridCollectionView : CollectionView
    {
        #region -- Public properties --

        public static readonly BindableProperty MinimumVisibleRowsNumberProperty = BindableProperty.Create(
            propertyName: nameof(MinimumVisibleRowsNumber),
            returnType: typeof(int),
            declaringType: typeof(ExpandedGridCollectionView));

        public int MinimumVisibleRowsNumber
        {
            get => (int)GetValue(MinimumVisibleRowsNumberProperty);
            set => SetValue(MinimumVisibleRowsNumberProperty, value);
        }

        public static readonly BindableProperty MaximumVisibleRowsNumberProperty = BindableProperty.Create(
            propertyName: nameof(MaximumVisibleRowsNumber),
            returnType: typeof(int),
            declaringType: typeof(ExpandedGridCollectionView));

        public int MaximumVisibleRowsNumber
        {
            get => (int)GetValue(MaximumVisibleRowsNumberProperty);
            set => SetValue(MaximumVisibleRowsNumberProperty, value);
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
                or nameof(MinimumVisibleRowsNumber)
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
                int visibleRowsNumber = MinimumVisibleRowsNumber;

                if (IsExpanded)
                {
                    int visibleRowsNumberIncludingColumns = (int)Math.Ceiling((double)((ICollection)ItemsSource).Count / gridItemsLayout.Span);

                    visibleRowsNumber = visibleRowsNumberIncludingColumns < MaximumVisibleRowsNumber
                        ? visibleRowsNumberIncludingColumns
                        : MaximumVisibleRowsNumber;
                }

                HeightRequest = MinimumHeightRequest = visibleRowsNumber * (itemTemplate.HeightRequest + gridItemsLayout.VerticalItemSpacing);
            }
        }

        #endregion
    }
}
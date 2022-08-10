using Next2.Enums;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls
{
    public class CustomCollectionView : CollectionView
    {
        private double _viewItemHeight;
        private double _viewItemWidth;

        public CustomCollectionView()
        {
            Scrolled += OnCollectionScrolled;
        }

        #region -- Public properties --

        public static readonly BindableProperty IndexLastVisibleProperty = BindableProperty.Create(
            propertyName: nameof(IndexLastVisible),
            returnType: typeof(int),
            declaringType: typeof(CustomCollectionView),
            defaultBindingMode: BindingMode.OneWayToSource);

        public int IndexLastVisible
        {
            get => (int)GetValue(IndexLastVisibleProperty);
            set => SetValue(IndexLastVisibleProperty, value);
        }

        public static readonly BindableProperty BounceModeProperty = BindableProperty.Create(
            propertyName: nameof(BounceMode),
            declaringType: typeof(CustomCollectionView),
            returnType: typeof(EBounceMode),
            defaultBindingMode: BindingMode.OneWay);

        public EBounceMode BounceMode
        {
            get => (EBounceMode)GetValue(BounceModeProperty);
            set => SetValue(BounceModeProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(ItemTemplate) or nameof(ItemsSource))
            {
                if (ItemTemplate is not null && ItemTemplate.CreateContent() is View view)
                {
                    _viewItemWidth = view.WidthRequest;
                    _viewItemHeight = view.HeightRequest;
                }
                else
                {
                    _viewItemWidth = 0;
                    _viewItemHeight = 0;
                    IndexLastVisible = -1;
                }
            }
        }

        #endregion

        #region -- Private helpers --

        private void OnCollectionScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            if (ItemsLayout is ItemsLayout layout)
            {
                if (layout.Orientation == ItemsLayoutOrientation.Vertical)
                {
                    if (_viewItemHeight > 0)
                    {
                        IndexLastVisible = (int)((Height + e.VerticalOffset) / _viewItemHeight);
                    }
                    else
                    {
                        IndexLastVisible = -1;
                    }
                }
                else
                {
                    if (_viewItemWidth > 0)
                    {
                        IndexLastVisible = (int)((Width + e.HorizontalOffset) / _viewItemWidth);
                    }
                    else
                    {
                        IndexLastVisible = -1;
                    }
                }
            }
        }

        #endregion
    }
}

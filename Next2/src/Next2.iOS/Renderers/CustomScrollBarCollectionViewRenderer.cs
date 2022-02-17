using Next2.Controls;
using Next2.iOS.Renderers;
using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomScrollBarCollectionView), typeof(CustomScrollBarCollectionViewRenderer))]
namespace Next2.iOS.Renderers
{
    class CustomScrollBarCollectionViewRenderer : CollectionViewRenderer
    {
        protected CustomScrollBarCollectionView _customScrollBarCollectionViewElement;
        protected CustomScrollBarCollectionView CustomScrollBarCollectionViewElement =>
            _customScrollBarCollectionViewElement == null
            ? _customScrollBarCollectionViewElement = (Element as CustomScrollBarCollectionView)
            : _customScrollBarCollectionViewElement;

        protected UIScrollView _view;
        protected UIColor _scrollBarThumbColor;

        protected override void OnElementChanged(ElementChangedEventArgs<GroupableItemsView> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                try
                {
                    _view = (Control as UIScrollView);
                    _view.Scrolled += Container_Scrolled;

                    if (CustomScrollBarCollectionViewElement != null)
                    {
                        _scrollBarThumbColor = CustomScrollBarCollectionViewElement.ScrollBarThumbColor.ToUIColor();
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void Container_Scrolled(object sender, System.EventArgs e)
        {
            var subViews = _view.Subviews.ToList();
            var verticalIndicator = subViews.LastOrDefault();

            if (verticalIndicator != null)
            {
                verticalIndicator.BackgroundColor = _scrollBarThumbColor;
            }
        }
    }
}
using Next2.Controls;
using Next2.Enums;
using Next2.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BouncingCollectionView), typeof(BouncingCollectionViewRenderer))]
namespace Next2.iOS.Renderers
{
    public class BouncingCollectionViewRenderer : CollectionViewRenderer
    {
        protected BouncingCollectionView _bouncingCollectionView;
        protected BouncingCollectionView BouncingCollectionView => _bouncingCollectionView ?? (_bouncingCollectionView = (BouncingCollectionView)Element);

        public BouncingCollectionViewRenderer()
        {
        }

        #region -- Overrides --

        protected override void OnElementChanged(ElementChangedEventArgs<GroupableItemsView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                DisableBounces();
            }
        }

        #endregion

        #region -- Protected helpers --

        protected void DisableBounces()
        {
            var overScollMode = _bouncingCollectionView.DisableBounceMode;

            Controller.CollectionView.Bounces = overScollMode == EDisableBounceMode.iOSOnly || overScollMode == EDisableBounceMode.Always;
        } 

        #endregion
    }
}
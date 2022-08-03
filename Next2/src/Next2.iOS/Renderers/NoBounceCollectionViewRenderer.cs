using Next2.Controls;
using Next2.Enums;
using Next2.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NoBounceCollectionView), typeof(NoBounceCollectionViewRenderer))]
namespace Next2.iOS.Renderers
{
    public class NoBounceCollectionViewRenderer : CollectionViewRenderer
    {
        public NoBounceCollectionViewRenderer()
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
            var disableBounceMode = (Element as NoBounceCollectionView).DisableBounceMode;

            Controller.CollectionView.Bounces = !(disableBounceMode == EDisableBounceMode.iOSOnly || disableBounceMode == EDisableBounceMode.All);
        } 

        #endregion
    }
}
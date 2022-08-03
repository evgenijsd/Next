using Next2.Controls;
using Next2.Enums;
using Next2.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomCollectionView), typeof(CustomCollectionViewRenderer))]
namespace Next2.iOS.Renderers
{
    public class CustomCollectionViewRenderer : CollectionViewRenderer
    {
        public CustomCollectionViewRenderer()
        {
        }

        #region -- Overrides --

        protected override void OnElementChanged(ElementChangedEventArgs<GroupableItemsView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                UpdateBounces();
            }
        }

        #endregion

        #region -- Protected helpers --

        protected void UpdateBounces()
        {
            var bounceMode = (Element as CustomCollectionView).BounceMode;

            Controller.CollectionView.Bounces = !(bounceMode == EBounceMode.DisabledForiOS || bounceMode == EBounceMode.DisabledForAndroidAndiOS);
        } 

        #endregion
    }
}
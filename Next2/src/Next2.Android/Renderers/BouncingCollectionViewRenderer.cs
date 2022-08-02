using Android.Content;
using Next2.Controls;
using Next2.Droid.Renderers;
using Next2.Enums;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BouncingCollectionView), typeof(BouncingCollectionViewRenderer))]
namespace Next2.Droid.Renderers
{
    public class BouncingCollectionViewRenderer : CollectionViewRenderer
    {
        protected BouncingCollectionView _bouncingCollectionView;
        protected BouncingCollectionView BouncingCollectionView => _bouncingCollectionView ??= (BouncingCollectionView)Element;

        public BouncingCollectionViewRenderer(Context context)
            : base(context)
        {
        }

        #region -- Overrides --

        protected override void OnElementChanged(ElementChangedEventArgs<ItemsView> elementChangedEvent)
        {
            base.OnElementChanged(elementChangedEvent);

            if (elementChangedEvent.NewElement != null)
            {
                DisableBounces();
            }
        }

        #endregion

        #region -- Protected helpers --

        protected void DisableBounces()
        {
            if (BouncingCollectionView.DisableBounceMode == EDisableBounceMode.AndroidOnly || BouncingCollectionView.DisableBounceMode == EDisableBounceMode.Always)
            {
                OverScrollMode = Android.Views.OverScrollMode.Never;
            }
            else
            {
                OverScrollMode = Android.Views.OverScrollMode.Always;
            }
        } 

        #endregion
    }
}
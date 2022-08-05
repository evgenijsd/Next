using Android.Content;
using Next2.Controls;
using Next2.Droid.Renderers;
using Next2.Enums;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NoBounceCollectionView), typeof(NoBounceCollectionViewRenderer))]
namespace Next2.Droid.Renderers
{
    public class NoBounceCollectionViewRenderer : CollectionViewRenderer
    {
        protected NoBounceCollectionView _noBounceCollectionView;
        protected NoBounceCollectionView NoBounceCollectionView => _noBounceCollectionView ??= (NoBounceCollectionView)Element;

        public NoBounceCollectionViewRenderer(Context context)
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
            OverScrollMode = NoBounceCollectionView.DisableBounceMode == EDisableBounceMode.AndroidOnly || NoBounceCollectionView.DisableBounceMode == EDisableBounceMode.All
                ? OverScrollMode = Android.Views.OverScrollMode.Never
                : OverScrollMode = Android.Views.OverScrollMode.Always;
        }

        #endregion
    }
}
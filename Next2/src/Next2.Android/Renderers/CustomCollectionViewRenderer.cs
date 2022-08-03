using Android.Content;
using Next2.Controls;
using Next2.Droid.Renderers;
using Next2.Enums;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomCollectionView), typeof(CustomCollectionViewRenderer))]
namespace Next2.Droid.Renderers
{
    public class CustomCollectionViewRenderer : CollectionViewRenderer
    {
        protected CustomCollectionView _customCollectionView;
        protected CustomCollectionView CustomCollectionView => _customCollectionView ??= (CustomCollectionView)Element;

        public CustomCollectionViewRenderer(Context context)
            : base(context)
        {
        }

        #region -- Overrides --

        protected override void OnElementChanged(ElementChangedEventArgs<ItemsView> elementChangedEvent)
        {
            base.OnElementChanged(elementChangedEvent);

            if (elementChangedEvent.NewElement != null)
            {
                UpdateOverScrollMode();
            }
        }

        #endregion

        #region -- Protected helpers --

        protected void UpdateOverScrollMode()
        {
            OverScrollMode = CustomCollectionView.BounceMode == EBounceMode.DisabledForAndroid || CustomCollectionView.BounceMode == EBounceMode.DisabledForAndroidAndiOS
                ? OverScrollMode = Android.Views.OverScrollMode.Never
                : OverScrollMode = Android.Views.OverScrollMode.Always;
        }

        #endregion
    }
}
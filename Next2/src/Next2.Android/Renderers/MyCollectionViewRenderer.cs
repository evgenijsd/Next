using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Widget;
using Next2.Controls;
using Next2.Droid.Renderers;
using Android.Graphics;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System;

[assembly: ExportRenderer(typeof(MyCollectionView), typeof(MyCollectionViewRenderer))]
namespace Next2.Droid.Renderers
{
    public class MyCollectionViewRenderer : CollectionViewRenderer
    {
        protected MyCollectionView _myCollectionView;
        protected MyCollectionView MyCollectionView => _myCollectionView ??= (Element as MyCollectionView);

        public MyCollectionViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
        {
            base.OnElementPropertyChanged(sender, changedProperty);

            this.ScrollBarSize = MyCollectionView.ScrollBarWidth;

            ShapeDrawable shapeDrawable = new ShapeDrawable();

            try
            {
                //Drawable d = getResources().getDrawable(android.R.drawable.ic_dialog_email);
                //ImageView image = (ImageView)findViewById(R.id.image);
                //image.setImageDrawable(d);

                //IntPtr IntPtrtextViewClass = JNIEnv.FindClass(typeof(TextView));
                //IntPtr mCursorDrawableResProperty = JNIEnv.GetFieldID(IntPtrtextViewClass, "scrollbar_vertical_track", "I");
                //JNIEnv.SetField(Control.Handle, mCursorDrawableResProperty, Resource.Drawable.scrollbar_vertical_track);
                Android.Graphics.Color color = MyCollectionView.ScrollColor.ToAndroid();
                //Android.Graphics.Color color = Android.Graphics.Color.Red;
                shapeDrawable.SetColorFilter(color, PorterDuff.Mode.SrcIn);
            }
            catch (Exception e)
            {
                shapeDrawable.SetColorFilter(Resources.GetColor(Android.Resource.Color.HoloGreenLight), PorterDuff.Mode.SrcIn);
            }

            this.VerticalScrollbarThumbDrawable = shapeDrawable;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ItemsView> elementChangedEvent)
        {
            base.OnElementChanged(elementChangedEvent);

        }
    }
}
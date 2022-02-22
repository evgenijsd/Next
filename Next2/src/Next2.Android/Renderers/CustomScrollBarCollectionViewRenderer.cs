using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Google.Android.Material.Shape;
using Next2.Controls;
using Java.Lang;
using Next2.Droid.Renderers;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;

namespace Next2.Droid.Renderers
{
    public class CustomScrollBarCollectionViewRenderer : CollectionViewRenderer
    {
        public CustomScrollBarCollectionViewRenderer(Context context)
            : base(context)
        {
        }
    }
}
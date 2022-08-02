using Next2.Effects;
using Next2.Enums;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Next2.Controls
{
    public partial class DrawPanel : Grid
    {
        private SKBitmap _bitmap;
        private List<SKPath> _completedPaths = new List<SKPath>();
        private Dictionary<long, SKPath> _inProgressPaths = new Dictionary<long, SKPath>();

        private SKPaint _paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 2,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
        };

        public DrawPanel()
        {
            InitializeComponent();
        }

        #region -- Public properties --

        public static readonly BindableProperty IsClearedProperty = BindableProperty.Create(
            propertyName: nameof(IsCleared),
            returnType: typeof(bool),
            declaringType: typeof(DrawPanel),
            defaultValue: true,
            defaultBindingMode: BindingMode.TwoWay);

        public bool IsCleared
        {
            get => (bool)GetValue(IsClearedProperty);
            set => SetValue(IsClearedProperty, value);
        }

        public static readonly BindableProperty BitmapProperty = BindableProperty.Create(
            propertyName: nameof(Bitmap),
            returnType: typeof(byte[]),
            declaringType: typeof(DrawPanel),
            defaultBindingMode: BindingMode.OneWayToSource);

        public byte[] Bitmap
        {
            get => (byte[])GetValue(BitmapProperty);
            set => SetValue(BitmapProperty, value);
        }

        #endregion

        #region -- Overrides --

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(IsCleared))
            {
                if (IsCleared)
                {
                    Clear();
                }
            }
        }

        #endregion

        #region -- Private helpers --

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            if (_bitmap == null)
            {
                _bitmap = new SKBitmap(info.Width, info.Height);
            }
            else if (_bitmap.Width < info.Width || _bitmap.Height < info.Height)
            {
                SKBitmap newBitmap = new SKBitmap(
                    Math.Max(_bitmap.Width, info.Width),
                    Math.Max(_bitmap.Height, info.Height));

                using (SKCanvas newCanvas = new SKCanvas(newBitmap))
                {
                    newCanvas.Clear();
                    newCanvas.DrawBitmap(_bitmap, 0, 0);
                }

                _bitmap = newBitmap;
            }

            canvas.Clear();
            canvas.DrawBitmap(_bitmap, 0, 0);
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case ETouchActionType.Pressed:
                    if (!_inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        _inProgressPaths.Add(args.Id, path);

                        IsCleared = false;
                    }

                    break;

                case ETouchActionType.Moved:
                    if (_inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = _inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                    }

                    break;

                case ETouchActionType.Released:
                    if (_inProgressPaths.ContainsKey(args.Id))
                    {
                        _completedPaths.Add(_inProgressPaths[args.Id]);
                        _inProgressPaths.Remove(args.Id);
                    }

                    break;

                case ETouchActionType.Cancelled:
                    if (_inProgressPaths.ContainsKey(args.Id))
                    {
                        _inProgressPaths.Remove(args.Id);
                    }

                    break;
            }

            UpdateBitmap();
        }

        private SKPoint ConvertToPixel(Point pt)
        {
            return new SKPoint(
                (float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        private void UpdateBitmap()
        {
            using (SKCanvas saveBitmapCanvas = new SKCanvas(_bitmap))
            {
                saveBitmapCanvas.Clear();

                foreach (SKPath path in _completedPaths)
                {
                    saveBitmapCanvas.DrawPath(path, _paint);
                }

                foreach (SKPath path in _inProgressPaths.Values)
                {
                    saveBitmapCanvas.DrawPath(path, _paint);
                }
            }

            canvasView.InvalidateSurface();

            Bitmap = _bitmap.Bytes;
        }

        private void Clear()
        {
            _completedPaths.Clear();
            _inProgressPaths.Clear();

            UpdateBitmap();

            canvasView.InvalidateSurface();

            Bitmap = _bitmap.Bytes;
        }

        #endregion
    }
}
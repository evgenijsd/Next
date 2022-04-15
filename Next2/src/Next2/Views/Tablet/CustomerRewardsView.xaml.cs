using Next2.Effects;
using Next2.Enums;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Next2.Views.Tablet
{
    public partial class CustomerRewardsView : BaseContentPage
    {
        private Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        private List<SKPath> completedPaths = new List<SKPath>();

        private SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 2,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
        };

        private SKBitmap saveBitmap;

        public CustomerRewardsView()
        {
            InitializeComponent();
        }

        private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            // Create bitmap the size of the display surface
            if (saveBitmap == null)
            {
                saveBitmap = new SKBitmap(info.Width, info.Height);
            }

            // Or create new bitmap for a new size of display surface
            else if (saveBitmap.Width < info.Width || saveBitmap.Height < info.Height)
            {
                SKBitmap newBitmap = new SKBitmap(
                    Math.Max(saveBitmap.Width, info.Width),
                    Math.Max(saveBitmap.Height, info.Height));

                using (SKCanvas newCanvas = new SKCanvas(newBitmap))
                {
                    newCanvas.Clear();
                    newCanvas.DrawBitmap(saveBitmap, 0, 0);
                }

                saveBitmap = newBitmap;
            }

            // Render the bitmap
            canvas.Clear();
            canvas.DrawBitmap(saveBitmap, 0, 0);
        }

        private void OnTouchEffectAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case ETouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        inProgressPaths.Add(args.Id, path);
                    }

                    break;

                case ETouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                    }

                    break;

                case ETouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);
                    }

                    break;

                case ETouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
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
            using (SKCanvas saveBitmapCanvas = new SKCanvas(saveBitmap))
            {
                saveBitmapCanvas.Clear();

                foreach (SKPath path in completedPaths)
                {
                    saveBitmapCanvas.DrawPath(path, paint);
                }

                foreach (SKPath path in inProgressPaths.Values)
                {
                    saveBitmapCanvas.DrawPath(path, paint);
                }
            }

            canvasView.InvalidateSurface();
        }

        private void OnClearButtonClicked(object sender, EventArgs args)
        {
            completedPaths.Clear();
            inProgressPaths.Clear();
            UpdateBitmap();
            canvasView.InvalidateSurface();
        }

        private async void OnSaveButtonClicked(object sender, EventArgs args)
        {
            //using (SKImage image = SKImage.FromBitmap(saveBitmap))
            //{
            //    SKData data = image.Encode();
            //    DateTime dt = DateTime.Now;
            //    string filename = String.Format("FingerPaint-{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}{5:D2}{6:D3}.png",
            //                                    dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

            //    IPhotoLibrary photoLibrary = DependencyService.Get<IPhotoLibrary>();
            //    bool result = await photoLibrary.SavePhotoAsync(data.ToArray(), "FingerPaint", filename);

            //    if (!result)
            //    {
            //        await DisplayAlert("FingerPaint", "Artwork could not be saved. Sorry!", "OK");
            //    }
            //}
        }
    }
}
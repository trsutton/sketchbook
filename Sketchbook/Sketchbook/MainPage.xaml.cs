using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace Sketchbook
{
    public partial class MainPage : ContentPage
    {
        private readonly object ctx = new object();

        public MainPage()
        {
            InitializeComponent();
        }

        public SKPath ActivePath { get; private set; }

        public List<SKPath> Paths { get; } = new List<SKPath>();

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;

            var strokePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 10
            };

            SKPath activePath = null;
            
            lock(ctx) 
            { 
                activePath = ActivePath; 
            }

            canvas.Clear();

            foreach(var path in Paths)
            {
                canvas.DrawPath(path, strokePaint);
            }

            if (activePath != null)
                canvas.DrawPath(activePath, strokePaint);
        }

        private void OnCanvasTouchEvent(object sender, SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    lock(ctx)
                    {
                        ActivePath = new SKPath();
                        ActivePath.MoveTo(e.Location);
                    }

                    break;

                case SKTouchAction.Moved:
                    lock(ctx)
                    {
                        ActivePath?.LineTo(e.Location);
                    }

                    break;

                case SKTouchAction.Released:
                    lock(ctx)
                    {
                        if (ActivePath != null)
                            Paths.Add(ActivePath);
                        
                        ActivePath = null;
                    }

                    break;
            }

            e.Handled = true;

            if (sender is SKCanvasView canvas)
                canvas.InvalidateSurface();
        }
    }
}

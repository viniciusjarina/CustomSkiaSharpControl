using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
using SkiaSharp;


namespace CustomSkiaControl
{
	[Register ("GradientBall"), DesignTimeVisible (true)]
	public class GradientBall : UIView
	{
		private SKCGSurfaceFactory drawable;

		[Export ("InnerBallColor"), Browsable (true)]
		public UIColor InnerBallColor { get; set; } = UIColor.White;

		[Export ("OutterBallColor"), Browsable (true)]
		public UIColor OutterBallColor { get; set; } = UIColor.FromRGB (0.910286247730255f, 0.976427972316742f, 1);

		[Export ("TopColor"), Browsable (true)]
		public UIColor TopColor { get; set; } = UIColor.FromRGB (0.066558399770476f, 0.122641354382306f, 1);

		[Export ("BottomColor"), Browsable (true)]
		public UIColor BottomColor { get; set; } = UIColor.FromRGB (0.862184178412046f, 0.901110125092369f, 1);

		public GradientBall (IntPtr handle) : base (handle) { }

		public GradientBall ()
		{
			// Called when created from code.
			Initialize ();
		}

		public override void AwakeFromNib ()
		{
			// Called when loaded from xib or storyboard.
			Initialize ();
		}

		void Initialize ()
		{
			// Common initialization code here.
			drawable = new SKCGSurfaceFactory ();
		}

		public SKSize CanvasSize => drawable?.Info.Size ?? SKSize.Empty;

		static SKColor FromUIColor (UIColor color)
		{
			nfloat red;
			nfloat green;
			nfloat blue;
			nfloat alpha;
			color.GetRGBA (out red, out green, out blue, out alpha);
			return new SKColor ((byte)(red * 255), (byte)(green * 255), (byte)(blue * 255), (byte) (alpha * 255));
		}


		public void DoDraw (SKCanvas canvas)
		{
			int width = (int)Bounds.Width;
			int height = (int)Bounds.Height;

			var bottomColor = FromUIColor (BottomColor);
			var topColor = FromUIColor (TopColor);

			var innerColor = FromUIColor (InnerBallColor);
			var outterColor = FromUIColor (OutterBallColor);

			using (var paint = new SKPaint ()) {
				paint.IsAntialias = true;
				using (var shader = SKShader.CreateLinearGradient (
					new SKPoint (0, 0),
					new SKPoint (0, height),
					new[] { topColor, bottomColor },
					null,
					SKShaderTileMode.Clamp)) {

					paint.Shader = shader;
					canvas.DrawPaint (paint);
				}
			}

			// Center and Scale the Surface
			var scale = (width < height ? width : height) / (240f);
			canvas.Translate (width / 2f, height / 2f);
			canvas.Scale (scale, scale);
			canvas.Translate (-128, -128);

			using (var paint = new SKPaint ()) {
				paint.IsAntialias = true;
				using (var shader = SKShader.CreateTwoPointConicalGradient (
					new SKPoint (115.2f, 102.4f),
					25.6f,
					new SKPoint (102.4f, 102.4f),
					128.0f,
					new[] { innerColor, outterColor },
					null,
					SKShaderTileMode.Clamp
				)) {
					paint.Shader = shader;
					canvas.DrawOval (new SKRect (51.2f, 51.2f, 204.8f, 204.8f), paint);
				}
			}
		}

		public override void Draw (CGRect rect)
		{
			base.Draw (rect);

			var screenScale = UIScreen.MainScreen.Scale;
			var width = (int)(Bounds.Width * screenScale);
			var height = (int)(Bounds.Height * screenScale);

			using (var ctx = UIGraphics.GetCurrentContext ()) {
				// create the skia context
				SKImageInfo info;
				using (var surface = drawable.CreateSurface (Bounds, ContentScaleFactor, out info)) {
					// draw on the image using SKiaSharp
					DoDraw (surface.Canvas);
					// draw the surface to the context
					drawable.DrawSurface (ctx, Bounds, info, surface);
				}
			}
		}
	}
}

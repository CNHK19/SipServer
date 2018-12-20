// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.ComponentModel;
using Bitmap = System.Drawing.Bitmap;
using ImageAnimator = System.Drawing.ImageAnimator;

namespace RichTextBoxEx
{
	public class RectClass
	{
		public Rect Rect { get; set; }
	}

	public class AnimatedImage : 
		System.Windows.Controls.Image
    {
		private GifDecoder gifDecoder;
		private int gifFrame;
		private int gifTimerCounter;
		private int shartTimerCounter;
		private Point oldRootOffset;
		private bool inAnimationRect;

		private const int FrameTimerInterval = 25;
		private const int SharpTimerInterval = 1000;
		private static DispatcherTimer frameTimer;
		private static bool frameTimerProcessing;
		private static List<WeakReference> allImages;
		private static Dictionary<Bitmap, GifDecoder> gifDecodersCache;

		static AnimatedImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatedImage), new FrameworkPropertyMetadata(typeof(AnimatedImage)));

			allImages = new List<WeakReference>();
			gifDecodersCache = new Dictionary<Bitmap, GifDecoder>();

			frameTimer = new DispatcherTimer(DispatcherPriority.Render);
			frameTimer.Tick += FrameTimer_Tick;
			frameTimer.Interval = new TimeSpan(0, 0, 0, 0, FrameTimerInterval);
			frameTimer.Start();
		}

		public AnimatedImage()
		{
			Stretch = Stretch.None;
			SnapsToDevicePixels = true;
			shartTimerCounter = SharpTimerInterval;
			inAnimationRect = true;

			allImages.Add(new WeakReference(this));
		}

		#region AnimationRect, UpdateInAnimationRect

		public RectClass AnimationRect
		{
			get;
			set;
		}

		private void UpdateInAnimationRect(Point rootOffset)
		{
			if (Parent is System.Windows.Documents.InlineUIContainer)
			{
				inAnimationRect = AnimationRect.Rect.Contains(rootOffset) |
					AnimationRect.Rect.Contains(rootOffset.X + ActualWidth, rootOffset.Y + ActualHeight);
			}
		}

		#endregion

		#region FrameTimer_Tick

		private static void FrameTimer_Tick(object sender, EventArgs e)
		{
			if (frameTimerProcessing == false)
			{
				frameTimerProcessing = true;

				bool needRemove = false;
				foreach (var imageRef in allImages)
				{
					if (imageRef.IsAlive)
					{
						var image = imageRef.Target as AnimatedImage;
						if (image != null)
							image.NextFrame(FrameTimerInterval);
					}
					else
						needRemove = true;
				}

				if (needRemove)
					allImages.RemoveAll((weakRef) => { return !weakRef.IsAlive; });

				frameTimerProcessing = false;
			}
		}

		private void NextFrame(int interval)
		{
			if (/*IsVisible && */gifDecoder != null)
			{
				shartTimerCounter += interval;

				if (gifDecoder.Frames.Count > 1 && inAnimationRect)
				{
					gifTimerCounter += interval;

					if (gifDecoder.Delays[gifFrame] < gifTimerCounter + FrameTimerInterval / 2)
					{
						shartTimerCounter = 0;
						gifTimerCounter = 0;

						gifFrame = (gifFrame + 1) % gifDecoder.Frames.Count;
						InvalidateVisual();
					}
				}

				if (shartTimerCounter > SharpTimerInterval)
				{
					shartTimerCounter = 0;

					if (WasMoved() && inAnimationRect)
						InvalidateVisual();
				}
			}
		}

		private bool WasMoved()
		{
			PresentationSource presentationSource = PresentationSource.FromVisual(this);

			if (presentationSource != null)
			{
				Point newRootOffset = TransformToAncestor(presentationSource.RootVisual).Transform(new Point());

				UpdateInAnimationRect(newRootOffset);

				if (newRootOffset != oldRootOffset)
				{
					oldRootOffset = newRootOffset;
					return true;
				}
			}

			return false;
		}

		#endregion

		#region Property AnimatedBitmap, Source

		public static readonly DependencyProperty AnimatedBitmapProperty = DependencyProperty.Register(
				"AnimatedBitmap", typeof(Bitmap), typeof(AnimatedImage),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAnimatedBitmapChanged)));

//		public static readonly DependencyProperty AnimatedBitmapUriProperty = DependencyProperty.Register(
//				"AnimatedBitmapUri", typeof(Bitmap), typeof(AnimatedImage));
//
//		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
//		public Uri AnimatedBitmapUri
//		{
//			get { return (Uri)GetValue(AnimatedBitmapUriProperty); }
//			set { SetValue(AnimatedBitmapUriProperty, value); }
//		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Bitmap AnimatedBitmap
        {
            get { return (Bitmap)GetValue(AnimatedBitmapProperty); }
            set { SetValue(AnimatedBitmapProperty, value); }
        }

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageSource Source 
		{
			get { return gifDecoder.Frames[gifFrame]; }
			set { /*base.Source = value;*/ } 
		}

		#endregion

		#region Event AnimatedBitmapChanged

		public static readonly RoutedEvent AnimatedBitmapChangedEvent = EventManager.RegisterRoutedEvent(
			"AnimatedBitmapChanged", RoutingStrategy.Bubble,
			typeof(RoutedPropertyChangedEventHandler<Bitmap>), typeof(AnimatedImage));

		private static void OnAnimatedBitmapChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			AnimatedImage control = (AnimatedImage)obj;

			control.UpdateAnimatedBitmap();

			control.OnAnimatedBitmapChanged(
				new RoutedPropertyChangedEventArgs<Bitmap>(
					(Bitmap)args.OldValue, (Bitmap)args.NewValue, AnimatedBitmapChangedEvent)
				);
		}

		public event RoutedPropertyChangedEventHandler<Bitmap> AnimatedBitmapChanged
        {
            add { AddHandler(AnimatedBitmapChangedEvent, value); }
            remove { RemoveHandler(AnimatedBitmapChangedEvent, value); }
        }

		protected virtual void OnAnimatedBitmapChanged(RoutedPropertyChangedEventArgs<Bitmap> args)
		{
			RaiseEvent(args);
		}

		#endregion

		#region UpdateAnimatedBitmap

		private void UpdateAnimatedBitmap()
		{
			if (AnimatedBitmap != null)
			{
				if (gifDecodersCache.TryGetValue(AnimatedBitmap, out gifDecoder) == false)
				{
					gifDecoder = new GifDecoder()
					{
						GifBitmap = AnimatedBitmap,
					};

					gifDecodersCache.Add(AnimatedBitmap, gifDecoder);
				}

				gifFrame = 0;
				gifTimerCounter = 0;

				base.Source = gifDecoder.Frames[gifFrame];
				InvalidateMeasure();
				InvalidateVisual();
			}
		}

		#endregion

		#region Rendering...

		public Size GetCurrentFrameSize()
		{
			if (gifDecoder == null)
				return new Size();

			var source = gifDecoder.Frames[gifFrame];
			return new Size(source.Width, source.Height);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			if (gifDecoder == null)
				return new Size();
			
			var source = gifDecoder.Frames[gifFrame];
			return new Size(source.Width, source.Height);
		}

		protected override void OnRender(DrawingContext dc)
		{
			if (gifDecoder != null)
			{
				ImageSource bitmapSource = gifDecoder.Frames[gifFrame];
				if (bitmapSource != null)
				{
					var pixelOffset = GetPixelOffset();

					dc.DrawImage(bitmapSource, new Rect(pixelOffset, DesiredSize));
				}
			}
		}

		private Point GetPixelOffset()
		{
			var offset = new Point();

			PresentationSource presentationSource = PresentationSource.FromVisual(this);
			if (presentationSource != null)
			{
				var rootVisual = presentationSource.RootVisual;

				offset = TransformToAncestor(rootVisual).Transform(offset);
				UpdateInAnimationRect(offset);
				offset = presentationSource.CompositionTarget.TransformToDevice.Transform(offset);

				offset.X = Math.Round(offset.X);
				offset.Y = Math.Round(offset.Y);

				offset = presentationSource.CompositionTarget.TransformFromDevice.Transform(offset);
				offset = rootVisual.TransformToDescendant(this).Transform(offset);
			}

			return offset;
		}

		#endregion
	}
}

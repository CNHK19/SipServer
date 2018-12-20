// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;

using Bitmap = System.Drawing.Bitmap;
using FrameDimension = System.Drawing.Imaging.FrameDimension;

namespace RichTextBoxEx
{
	public class GifDecoder
	{
		private Bitmap gifBitmap;

		public const int MinDelay = 50;

		public Bitmap GifBitmap
		{
			get
			{
				return gifBitmap;
			}
			set
			{
				gifBitmap = value;

				Frames = null;
				Delays = null;

				if (gifBitmap != null)
				{
					var rawRepeat = gifBitmap.GetPropertyItem(0x5101).Value;
					if (rawRepeat != null)
						Repeat = BitConverter.ToUInt16(rawRepeat, 0) != 0;

					int framesCount = gifBitmap.GetFrameCount(FrameDimension.Time);

					if (framesCount > 0)
					{
						var delays = new int[framesCount];
						var frames = new BitmapSource[framesCount];

						byte[] rawDelays = gifBitmap.GetPropertyItem(0x5100).Value;

						for (int i = 0; i < framesCount; i++)
						{
							if (rawDelays != null && rawDelays.Length > i * 4)
							{
								delays[i] = BitConverter.ToInt32(rawDelays, i * 4) * 10;
								if (delays[i] == 0)
									delays[i] = 100;
								delays[i] = Math.Max(MinDelay, delays[i]);
							}

							gifBitmap.SelectActiveFrame(FrameDimension.Time, i);

							using (Bitmap bitmap = new Bitmap(gifBitmap))
							{
								bitmap.MakeTransparent();
								frames[i] = bitmap.CreateBitmapSource();
								//System.Windows.Media.RenderOptions.SetBitmapScalingMode(frames[i], System.Windows.Media.BitmapScalingMode.Unspecified);
								//System.Windows.Media.RenderOptions.SetCachingHint(frames[i], System.Windows.Media.CachingHint.Cache);
							}
						}

						Delays = new ReadOnlyCollection<int>(delays);
						Frames = new ReadOnlyCollection<BitmapSource>(frames);
					}
				}
			}
		}

		/// <summary>
		/// Play once or repeat foreever
		/// </summary>
		public bool Repeat
		{
			get;
			private set;
		}

		/// <summary>
		/// BitmapSources of all frames
		/// </summary>
		public ReadOnlyCollection<BitmapSource> Frames
		{
			get;
			private set;
		}

		/// <summary>
		/// Frame delays in milleseconds
		/// </summary>
		public ReadOnlyCollection<int> Delays
		{
			get;
			private set;
		}
	}

	public static class BitmapHelper
	{
		[DllImport("gdi32.dll")]
		private static extern bool DeleteObject(IntPtr hObject);

		public static BitmapSource CreateBitmapSource(this Bitmap bitmap)
		{
			IntPtr hBitmap = bitmap.GetHbitmap();

			try
			{
				return Imaging.CreateBitmapSourceFromHBitmap(
					hBitmap,
					IntPtr.Zero,
					Int32Rect.Empty,
					BitmapSizeOptions.FromEmptyOptions()
				);
			}
			finally
			{
				DeleteObject(hBitmap);
			}
		}
	}
}

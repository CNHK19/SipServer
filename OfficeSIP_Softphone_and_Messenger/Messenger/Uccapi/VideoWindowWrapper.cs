// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.ComponentModel;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
	enum VideoWindowAlign
	{
		TopLeft,
		BottomRight,
	}

	class VideoWindowWrapper
	{
		private IVideoWindow videoWindow;
		private IntPtr hwndParent;
		private bool isVisible = true;

		public double MaxWidth = double.MaxValue;
		public double MaxHeight = double.MaxValue;
		public int MaxWidthPx;
		public int MaxHeightPx;

		#region public IVideoWindow VideoWindow

		public IVideoWindow VideoWindow
		{
			get { return videoWindow; }
			set
			{
				if (videoWindow != value)
				{
					DetachVideoWindow();

					videoWindow = value;
				}
			}
		}

		#endregion

		#region public IntPtr HwndParent

		public IntPtr HwndParent
		{
			get { return hwndParent; }
			set
			{
				if (hwndParent != value)
					hwndParent = value;
			}
		}

		#endregion

		public bool IsVisible
		{
			get { return isVisible; }
			set
			{
				if (isVisible != value)
				{
					isVisible = value;
					SetVideoWindowVisible(isVisible);
				}
			}
		}

		public bool Attach()
		{
			if (hwndParent != IntPtr.Zero && videoWindow != null)
			{
				if (IsVideoWindowAttached() == false)
				{
					AttachVideoWindow(hwndParent);
					SetVideoWindowVisible(isVisible);
				}
				return true;
			}
			return false;
		}

		public bool IsMaxSizeChanged()
		{
			int newWidthPx, newHeightPx;
			GetMaxIdealImageSize(out newWidthPx, out newHeightPx);

			return newWidthPx != MaxWidthPx || newHeightPx != MaxHeightPx;
		}

		public void UpdateMaxSize(Matrix fromDevice)
		{
			GetMaxIdealImageSize(out MaxWidthPx, out MaxHeightPx);

			MaxWidth = (double)MaxWidthPx * fromDevice.M11;
			MaxHeight = (double)MaxHeightPx * fromDevice.M22;
		}

		public void UpdateSize(Rect rect, Matrix toDevice)
		{
			if (IsVideoWindowAttached())
			{
				int left = (int)(rect.Left * toDevice.M11);
				int top = (int)(rect.Top * toDevice.M22);
				int width = (int)(rect.Width * toDevice.M11);
				int height = (int)(rect.Height * toDevice.M22);

				SetVideoWindowSize(left, top, width, height);
			}
		}

		public Rect GetMaxProportionalSize(Rect available, VideoWindowAlign align, double percent)
		{
			if (available.Height <= 0 || available.Width <= 0)
				return new Rect(0, 0, 0, 0);

			if (MaxHeightPx == 0 || MaxWidthPx == 0)
				return new Rect(0, 0, 0, 0);

			double width1 = Math.Min(available.Width, MaxWidth);
			double height1 = width1 * (double)MaxHeightPx / (double)MaxWidthPx;

			if (height1 > Math.Min(available.Height, MaxHeight))
			{
				height1 = Math.Min(available.Height, MaxHeight);
				width1 = height1 * (double)MaxWidthPx / (double)MaxHeightPx;
			}

			double width2 = width1 * percent;
			double height2 = height1 * percent;

			double left1 = available.X;
			double top1 = available.Y;

			if (align == VideoWindowAlign.BottomRight)
			{
				left1 += width1 - width2;
				top1 += height1 - height2;
			}

			return new Rect(left1, top1, width2, height2);
		}

		#region VideoWindow Helpers

		private void AttachVideoWindow(IntPtr hwndHost)
		{
			try
			{
				if (videoWindow != null)
				{
					videoWindow.WindowStyle = 0x46000000;
					videoWindow.Owner = (int)hwndHost;
					videoWindow.MessageDrain = (int)hwndHost;
				}
			}
			catch
			{
				DetachVideoWindow();
			}
		}

		private void DetachVideoWindow()
		{
			if (videoWindow != null)
			{
				try
				{
					videoWindow.Visible = 0;
					videoWindow.Owner = 0;
					videoWindow.MessageDrain = 0;
				}
				catch { }

				videoWindow = null;
			}
		}

		private bool IsVideoWindowAttached()
		{
			try
			{
				if (videoWindow != null)
					return videoWindow.Owner != 0;
			}
			catch { }

			return false;
		}

		private void SetVideoWindowSize(int left, int top, int width, int height)
		{
			try
			{
				if (videoWindow != null)
				{
					if (videoWindow.Top != top)
						videoWindow.Top = top;

					if (videoWindow.Left != left)
						videoWindow.Left = left;

					if (videoWindow.Width != width)
						videoWindow.Width = width;

					if (videoWindow.Height != height)
						videoWindow.Height = height;
				}
			}
			catch { }
		}

		private void SetVideoWindowVisible(bool isVisible)
		{
			if (videoWindow != null)
			{
				try
				{
					if ((videoWindow.Visible != 0) != isVisible)
						videoWindow.Visible = isVisible ? -1 : 0;
				}
				catch
				{
					DetachVideoWindow();
				}
			}
		}

		private void GetMaxIdealImageSize(out int width, out int height)
		{
			width = height = 0;

			try
			{
				if (videoWindow != null)
					videoWindow.GetMaxIdealImageSize(out width, out height);
			}
			catch { }
		}

		#endregion
	}
}
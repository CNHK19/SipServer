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
	public class VideoWindowHost
		: HwndHost
		, INotifyPropertyChanged
	{
		private bool firstMeasure = true;

		private VideoWindowWrapper[] videoWindows = 
			new VideoWindowWrapper[] { new VideoWindowWrapper(), new VideoWindowWrapper(), };

		#region BuildWindowCore, DestroyWindowCore

		protected override HandleRef BuildWindowCore(HandleRef parent)
		{
			// WS_CHILD = 0x40000000
			// WS_VISIBLE = 0x10000000
			// WS_CLIPSIBLINGS = 0x04000000
			// WS_CLIPCHILDREN = 0x02000000
			IntPtr hwndHost = CreateWindowEx(0, "static", "",
							0x40000000 | 0x10000000 | 0x02000000,
							0, 0,
							320, 240,
							parent.Handle,
							IntPtr.Zero,
							IntPtr.Zero,
							0);

			foreach (var videoWindow in videoWindows)
				videoWindow.HwndParent = hwndHost;

			AttachVideoWindows();

			return new HandleRef(this, hwndHost);
		}

		protected override void DestroyWindowCore(HandleRef hwnd)
		{
			foreach (var videoWindow in videoWindows)
				videoWindow.VideoWindow = null;

			DestroyWindow(hwnd.Handle);
		}

		#endregion

		private void AttachVideoWindows()
		{
			foreach (var videoWindow in videoWindows)
				if (videoWindow.Attach() == false)
					break;
		}

		#region MeasureOverride, ArrangeOverride

		protected override Size MeasureOverride(Size available)
		{
			if (firstMeasure)
			{
				firstMeasure = false;

				if (VideoSize.IsEmpty || VideoSize.Width <= 0 || VideoSize.Height <= 0)
					VideoSize = new Size(Math.Min(360, available.Width), Math.Min(300, available.Height));

				var ps = PresentationSource.FromVisual(this);
				var ct = (ps != null) ? ps.CompositionTarget : null;
				var matrix = (ct != null) ? ct.TransformFromDevice : new Matrix(1, 0, 0, 1, 0, 0);

				foreach (var videoWindow in videoWindows)
					videoWindow.UpdateMaxSize(matrix);

				Rect window0, window1;
				ArrangeVideoWindows(new Rect(VideoSize), out window0, out window1);

				window0.Union(window1);

				return window0.Size;
			}

			return base.MeasureOverride(available);
		}

		protected override Size ArrangeOverride(Size final)
		{
			Rect used = new Rect(0, 0, 0, 0);

			var ps = PresentationSource.FromVisual(this);
			var ct = (ps != null) ? ps.CompositionTarget : null;

			if (ct != null)
			{
				foreach (var videoWindow in videoWindows)
					videoWindow.UpdateMaxSize(ct.TransformFromDevice);

				if (videoWindows.Length == 2)
				{
					Rect window0, window1;
					ArrangeVideoWindows(new Rect(final), out window0, out window1);

					videoWindows[0].UpdateSize(window0, ct.TransformToDevice);
					videoWindows[1].UpdateSize(window1, ct.TransformToDevice);

					used = window0;
					used.Union(window1);
				}
				else
					throw new NotImplementedException();

				UpdateVideoSize(used);
			}

			return used.Size;
		}

		private void ArrangeVideoWindows(Rect available, out Rect window0, out Rect window1)
		{
			window0 = videoWindows[0].GetMaxProportionalSize(available, VideoWindowAlign.TopLeft, 1.0);

			Rect freeSpace1 = new Rect(window0.Width, 0, available.Width - window0.Width, available.Height);
			Rect freeSpace2 = new Rect(0, window0.Height, available.Width, available.Height - window0.Height);

			Rect size1Pip = videoWindows[1].GetMaxProportionalSize(window0, VideoWindowAlign.BottomRight, 0.33);
			Rect size1Near1 = videoWindows[1].GetMaxProportionalSize(freeSpace1, VideoWindowAlign.TopLeft, 1.0);
			Rect size1Near2 = videoWindows[1].GetMaxProportionalSize(freeSpace2, VideoWindowAlign.TopLeft, 1.0);

			if ((size1Pip.Width > size1Near1.Width && size1Pip.Width > size1Near2.Width) || videoWindows[1].IsVisible == false)
				window1 = size1Pip;
			else if (size1Near1.Width > size1Near2.Width)
				window1 = size1Near1;
			else
				window1 = size1Near2;
		}

		#endregion

		#region VideoSize

		public Size VideoSize { get; set; }

		private void UpdateVideoSize(Rect rect)
		{
			if (VideoSize.Width != rect.Width || VideoSize.Height != rect.Height)
			{
				VideoSize = rect.Size;
				OnPropertyChanged(@"VideoSize");
			}
		}

		#endregion

		#region VideoWindow1, VideoWindow2

		public IVideoWindow VideoWindow1
		{
			get { return videoWindows[0].VideoWindow; }
			set 
			{ 
				SetVideoWindow(0, value);

				Visibility = (value != null) ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public IVideoWindow VideoWindow2
		{
			get { return videoWindows[1].VideoWindow; }
			set { SetVideoWindow(1, value); }
		}

		private void SetVideoWindow(int i, IVideoWindow videoWindow)
		{
			if (videoWindows[i].VideoWindow != videoWindow)
			{
				videoWindows[i].VideoWindow = videoWindow;

				AttachVideoWindows();
				InvalidateArrange();
			}
		}

		#endregion

		#region IsVideo2Visible

		public bool IsVideo2Visible
		{
			get { return videoWindows[1].IsVisible; }
			set 
			{ 
				videoWindows[1].IsVisible = value;
				InvalidateArrange();
			}
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(String property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		#endregion INotifyPropertyChanged

		//	protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		//	{
		//		handled = false;
		//		return IntPtr.Zero;
		//	}

		#region Win32 Interop: CreateWindowEx, DestroyWindow

		[DllImport("user32.dll", EntryPoint = "CreateWindowEx", CharSet = CharSet.Auto)]
		internal static extern IntPtr CreateWindowEx(int dwExStyle, string lpszClassName, string lpszWindowName, int style, int x, int y, int width, int height, IntPtr hwndParent, IntPtr hMenu, IntPtr hInst, [MarshalAs(UnmanagedType.AsAny)] object pvParam);

		[DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Auto)]
		internal static extern bool DestroyWindow(IntPtr hwnd);

		#endregion
	}
}

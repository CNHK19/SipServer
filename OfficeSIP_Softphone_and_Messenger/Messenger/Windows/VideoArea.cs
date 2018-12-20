// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.ComponentModel;
using Uccapi;

namespace Messenger.Windows
{
	public class VideoArea
		: Decorator
		, IWeakEventListener
	{
		#region VideoSessionProperty, VideoMarginProperty, ViewLocalVideo, VideoSize

		public static readonly DependencyProperty VideoSessionProperty;
		public static readonly DependencyProperty VideoMarginProperty;
		public static readonly DependencyProperty ViewLocalVideoProperty;
		public static readonly DependencyProperty VideoSizeProperty;

		static VideoArea()
		{
			VideoSessionProperty = DependencyProperty.Register(@"VideoSession", typeof(IAvSession), typeof(VideoArea),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, VideoSessionChangedCallback));

			VideoMarginProperty = DependencyProperty.Register(@"VideoMargin", typeof(Thickness), typeof(VideoArea));

			ViewLocalVideoProperty = DependencyProperty.Register(@"ViewLocalVideo", typeof(bool), typeof(VideoArea),
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.None, ViewLocalVideoChangedCallback));

			VideoSizeProperty = DependencyProperty.Register(@"VideoSize", typeof(Size), typeof(VideoArea),
				new FrameworkPropertyMetadata(Size.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, VideoSizeChangedCallback));
		}

		private static void VideoSessionChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			(o as VideoArea).OnVideoSessionChanged(e.NewValue as IAvSession);
		}

		private static void ViewLocalVideoChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			(o as VideoArea).OnViewLocalVideoChanged((bool)e.NewValue);
		}

		private static void VideoSizeChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			(o as VideoArea).OnVideoSizeChanged((Size)e.NewValue);
		}

		public IAvSession VideoSession
		{
			set { SetValue(VideoSessionProperty, value); }
			get { return (IAvSession)GetValue(VideoSessionProperty); }
		}

		public Thickness VideoMargin
		{
			set { SetValue(VideoMarginProperty, value); }
			get { return (Thickness)GetValue(VideoMarginProperty); }
		}

		public bool ViewLocalVideo
		{
			set { SetValue(ViewLocalVideoProperty, value); }
			get { return (bool)GetValue(ViewLocalVideoProperty); }
		}

		public Size VideoSize
		{
			set { SetValue(VideoSizeProperty, value); }
			get { return (Size)GetValue(VideoSizeProperty); }
		}

		#endregion

		public VideoArea()
		{
		}

		private void OnVideoSessionChanged(IAvSession newSession)
		{
			Child = (newSession != null) ? newSession.VideoWindow : null;

			if (Child != null)
			{
				var videoHost = Child as VideoWindowHost;

				videoHost.Margin = VideoMargin;
				videoHost.IsVideo2Visible = ViewLocalVideo;
				videoHost.VideoSize = VideoSize;

				PropertyChangedEventManager.AddListener(videoHost, this, string.Empty);
			}
		}

		private void OnViewLocalVideoChanged(bool viewLocalVideo)
		{
			if (Child != null)
				(Child as VideoWindowHost).IsVideo2Visible = ViewLocalVideo;
		}

		private void OnVideoSizeChanged(Size videoSize)
		{
			if (Child != null)
				(Child as VideoWindowHost).VideoSize = videoSize;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (Child != null && e.PropertyName == @"VideoSize")
				VideoSize = (Child as VideoWindowHost).VideoSize;
		}

		#region IWeakEventListener

		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, Object sender, EventArgs e)
		{
			if (managerType == typeof(PropertyChangedEventManager))
			{
				OnPropertyChanged(sender, e as PropertyChangedEventArgs);
				return true;
			}

			return false;
		}

		#endregion
	}
}

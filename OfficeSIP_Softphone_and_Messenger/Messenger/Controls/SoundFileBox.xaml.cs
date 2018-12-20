// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Messenger.Controls
{
	/// <summary>
	/// Interaction logic for SoundFileBox.xaml
	/// </summary>
	public partial class SoundFileBox 
		: UserControl
	{
		public static readonly DependencyProperty SoundFileProperty =
			DependencyProperty.Register(@"SoundFile", typeof(string), typeof(SoundFileBox),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
		
		private MediaPlayer mediaPlayer;

		public SoundFileBox()
		{
			InitializeComponent();

			mediaPlayer = new MediaPlayer();
			this.Unloaded += new RoutedEventHandler(SoundFileBox_Unloaded);
		}

		void SoundFileBox_Unloaded(object sender, RoutedEventArgs e)
		{
			mediaPlayer.Stop();
		}
		
		public string SoundFile
		{
			get
			{
				return (string)this.GetValue(SoundFileProperty);
			}
			set
			{
				this.SetValue(SoundFileProperty, value);
			}
		}
		
		private void Select_Click(object sender, RoutedEventArgs e)
		{
			var openFileDialog = new System.Windows.Forms.OpenFileDialog();
			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				SoundFile = openFileDialog.FileName;
		}

		private void Play_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				mediaPlayer.Open(new Uri(System.IO.Path.GetFullPath(SoundFile)));
				mediaPlayer.Play();
			}
			catch
			{
			}
		}
	}
}

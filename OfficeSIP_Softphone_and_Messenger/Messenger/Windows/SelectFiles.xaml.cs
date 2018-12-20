// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using Uccapi;

namespace Messenger.Windows
{
	public partial class SelectFiles 
		: WindowEx
	{
		private List<TransferItemView> items;
		private ITransfersManager manager;
		private string downloadPath;
		private string fromUri;
		private string displayName;

		public SelectFiles(ITransfersManager manager1, IEnumerable<ITransferItem> items1, string fromUri1, string displayName1)
		{
			downloadPath = Messenger.Properties.Settings.Default.DownloadPath;
			if (string.IsNullOrEmpty(downloadPath))
			{
				StringBuilder path = new StringBuilder();
				if (Helpers.SHGetSpecialFolderPath(IntPtr.Zero, path, Helpers.CSIDL_PERSONAL, false))
					downloadPath = path.ToString();
			}

			manager = manager1;
			items = new List<TransferItemView>();
			foreach (var item1 in items1)
				items.Add(new TransferItemView(item1));
			UpdateFilesExist();

			fromUri = fromUri1;
			displayName = displayName1;

			InitializeComponent();

			Title = Title.Replace("AssemblyTitle", AssemblyInfo.AssemblyTitle);
		}

		public IEnumerable<TransferItemView> Files
		{
			get { return items; }
		}

		public string Username
		{
			get { return displayName; }
		}

		public string Uri
		{
			get { return fromUri; }
		}

		public string DownloadPath
		{
			get { return downloadPath; }
			set
			{
				if (value != downloadPath)
				{
					downloadPath = value;
					Messenger.Properties.Settings.Default.DownloadPath = value;
					OnPropertyChanged(@"DownloadPath");
					UpdateFilesExist();
				}
			}
		}

		private void UpdateFilesExist()
		{
			foreach (var file in Files)
				file.UpdateFileExist(downloadPath);
		}

		private void Accept_Click(object sender, RoutedEventArgs e)
		{
			List<ITransferItem> AcceptedItems = new List<ITransferItem>();
			List<ITransferItem> RejectedItems = new List<ITransferItem>();

			foreach (var item in items)
				if (item.IsAccepted)
					AcceptedItems.Add(item.Value);
				else
					RejectedItems.Add(item.Value);

			if (AcceptedItems.Count > 0)
				manager.Accept(DownloadPath, AcceptedItems);
			if (RejectedItems.Count > 0)
				manager.Reject(RejectedItems);

			Close();
		}

		private void Reject_Click(object sender, RoutedEventArgs e)
		{
			foreach (var item in items)
				manager.Reject(item.Value);

			Close();
		}

		private void Select_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new System.Windows.Forms.FolderBrowserDialog();
			dlg.SelectedPath = DownloadPath;
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				DownloadPath = dlg.SelectedPath;
		}

		#region class TransferItemView

		public class TransferItemView
			: DependencyObject
		{
			private ITransferItem item;
			public static readonly DependencyProperty FileExistProperty = DependencyProperty.Register("FileExist", typeof(string), typeof(TransferItemView));
			public static readonly DependencyProperty OldFileInfoProperty = DependencyProperty.Register("OldFileInfo", typeof(string), typeof(TransferItemView));

			public TransferItemView(ITransferItem item1)
			{
				item = item1;
				IsAccepted = true;
			}

			public bool IsAccepted { get; set; }

			public ITransferItem Value
			{
				get { return item; }
			}

			public string FileSize
			{
				get { return Helpers.SizeToStr(item.FileSize); }
			}

			public string FileExist
			{
				get { return (string)GetValue(FileExistProperty); }
			}

			public string OldFileInfo
			{
				get { return (string)GetValue(OldFileInfoProperty); }
			}

			public void UpdateFileExist(string path)
			{
				var pathName=Path.Combine(path, Value.FileName);

				if (File.Exists(pathName))
				{
					SetValue(FileExistProperty, "Yes");

					FileInfo fi = new FileInfo(pathName);

					SetValue(OldFileInfoProperty, "Old file information:\r\n" + fi.FullName
						+ "\r\nCreated: " + fi.CreationTime + "\r\nSize: "
						+ Helpers.SizeToStr(fi.Length));
				}
				else
				{
					SetValue(FileExistProperty, "");
					SetValue(OldFileInfoProperty, "");
				}
			}
		}

		#endregion
	}
}

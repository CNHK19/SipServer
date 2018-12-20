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
using System.Text.RegularExpressions;

namespace RichTextBoxExTest
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		private int textChangeCounter = 0;

		public Window1()
		{
			InitializeComponent();

			richex.TextChanged += Richex_TextChanged;

			this.Loaded += new RoutedEventHandler(Window1_Loaded);
		}

		void Window1_Loaded(object sender, RoutedEventArgs e)
		{
			richex.AppendAllEmoticons();
////			<tr class="alternate">
////				<td><img src="hiddenemoticons_files/75.gif" alt="yin yang"></td>
////				<td><kbd>(%)</kbd></td>
////				<td class="description"><span>yin yang</span></td>
////			</tr>
//
//            Regex regex1 = new Regex("<tr[^/]+/(?<gif>[^.]+)\\.gif\"[^\n]*\r\n\\s*<td><kbd>(?<smile>[^<]+)</kbd></td>\r\n\\s*<td class=\"description\"><span>(?<desc>[^<]+)", RegexOptions.Singleline);
//
//            var maches = regex1.Matches(Properties.Resources.yahoo);
//
//            console1.Text = maches.Count.ToString() + "\r\n";
//
//            if (maches.Count > 0)
//            {
//                for (int i = 0; i < maches.Count; i++ )
//                {
//                    console1.Text += 
//                        string.Format("new EmoticonDescription() {{ Smilez = @\"{1}\", Description=\"{2}\", Emoticon = Resources._{0}, }},\r\n"
//                            , maches[i].Groups["gif"].Value
//                            , Unhtml(maches[i].Groups["smile"].Value)
//                            , maches[i].Groups["desc"].Value);
//                }
//            }
		}

		string Unhtml(string source)
		{
			return source.Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
		}

		private void Richex_TextChanged(object sender, TextChangedEventArgs e)
		{
			textChangeCounter++;

			console1.Text = "";

			foreach (var change in e.Changes)
			{
				string added = "";

				if (change.AddedLength > 0)
				{
					var range = new TextRange(
						richex.Document.ContentStart.GetPositionAtOffset(change.Offset),
						richex.Document.ContentStart.GetPositionAtOffset(change.Offset + change.AddedLength)
						);

					added = range.Text;
				}

				if (change.RemovedLength > 0)
				{
				}

				console1.Text += string.Format("CHANGE: Offset:{0};    AddedLength:{1} '{3}';    RemovedLength:{2}\r\n",
					change.Offset, change.AddedLength, change.RemovedLength, added);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			richex.InvalidateMeasure();
			richex.InvalidateVisual();
		//	sm1.InvalidateMeasure();
		//	sm1.InvalidateVisual();
		}

		private void InsertSelected_Click(object sender, RoutedEventArgs e)
		{
			string rtf = richex.InsertDefaultFontInfo(richex.GetRtf(richex.Selection));
			dest.SetRtf(dest.Selection, rtf);
			console1.Text = rtf;
		}
	}
}

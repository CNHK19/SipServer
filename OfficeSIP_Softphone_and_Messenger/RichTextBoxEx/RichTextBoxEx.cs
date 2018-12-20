// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.IO;
using System.Text.RegularExpressions;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using FontFamily = System.Windows.Media.FontFamily;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

using System.Windows.Controls.Primitives;

namespace RichTextBoxEx
{
	public class RichTextBoxExCommands
	{
		public readonly static RoutedUICommand InsertText =
			new RoutedUICommand("InsertText", "InsertText", typeof(RichTextBoxExCommands));
		public readonly static RoutedUICommand SetFontFamily =
			new RoutedUICommand("SetFontFamily", "SetFontFamily", typeof(RichTextBoxExCommands));
		public readonly static RoutedUICommand SetForeground =
			new RoutedUICommand("SetForeground", "SetForeground", typeof(RichTextBoxExCommands));
		public readonly static RoutedUICommand SetDefaultFont =
			new RoutedUICommand("Set Default Font", "SetDefaultFont", typeof(RichTextBoxExCommands));

		public readonly static RoutedUICommand CtrlEnter =
			new RoutedUICommand("CtrlEnter", "CtrlEnter", typeof(RichTextBoxExCommands),
				CreateGestureCollection(Key.Enter, ModifierKeys.Control));

		static private InputGestureCollection CreateGestureCollection(Key key, ModifierKeys modifiers)
		{
			var gestureCollection = new InputGestureCollection();
			gestureCollection.Add(new KeyGesture(key, modifiers));

			return gestureCollection;
		}
	}

	public class RichTextBoxEx : RichTextBox
	{
		private RabinKarp<TextPointer, Bitmap> rabinKarp;
		private RectClass animationRect;
		private int spellCheckMenuIndex;

		public RichTextBoxEx()
		{
			IsAutoEmoticoning = true;
			spellCheckMenuIndex = 0;

			animationRect = new RectClass();

			InitializeRabinKarp();

			DataObject.AddCopyingHandler(this, NoDragCopy);

			CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, Execute_Copy, CanExecute_Copy));
			CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, Execute_Cut, CanExecute_Cut));
			CommandBindings.Add(new CommandBinding(RichTextBoxExCommands.InsertText, Execute_InsertText, CanExecute_InsertText));
			CommandBindings.Add(new CommandBinding(RichTextBoxExCommands.SetFontFamily, Execute_SetFontFamily, CanExecute_SetFontFamily));
			CommandBindings.Add(new CommandBinding(RichTextBoxExCommands.SetForeground, Execute_SetForeground, CanExecute_SetForeground));
			CommandBindings.Add(new CommandBinding(RichTextBoxExCommands.SetDefaultFont, Execute_SetDefaultFont));

			ContextMenu = new ContextMenu();
		}

		#region ConextMenu

		protected override void OnContextMenuOpening(ContextMenuEventArgs e)
		{
			base.OnContextMenuOpening(e);

			if (ContextMenu.Items.Count == 0)
			{
				if (IsReadOnly == false)
					ContextMenu.Items.Add(new MenuItem() { Command = ApplicationCommands.Cut, CommandTarget = this, });
				ContextMenu.Items.Add(new MenuItem() { Command = ApplicationCommands.Copy, CommandTarget = this, });
				ContextMenu.Items.Add(new MenuItem() { Command = ApplicationCommands.Paste, CommandTarget = this, });

				if (IsReadOnly == false)
				{
					ContextMenu.Items.Add(new Separator());
					ContextMenu.Items.Add(CreateMenuEmoticons(@"Smiles #1", YahooEmoticons.Emoticons, 0, 61));
					ContextMenu.Items.Add(CreateMenuEmoticons(@"Smiles #2", YahooEmoticons.Emoticons, 61, YahooEmoticons.Emoticons.Length));

					ContextMenu.Items.Add(new Separator());
					ContextMenu.Items.Add(CreateFormatMenu());
					ContextMenu.Items.Add(CreateFontSizeMenu());
					ContextMenu.Items.Add(CreateFontFamilyMenu());
					ContextMenu.Items.Add(CreateFontColorMenu(@"Foreground", RichTextBoxExCommands.SetForeground));

					ContextMenu.Items.Add(new Separator());
					ContextMenu.Items.Add(new MenuItem() { Command = RichTextBoxExCommands.SetDefaultFont, CommandTarget = this, });
				}
			}

			while (spellCheckMenuIndex > 0)
			{
				ContextMenu.Items.RemoveAt(0);
				spellCheckMenuIndex--;
			}

			if (SpellCheck.IsEnabled && Selection.IsEmpty && IsReadOnly == false)
			{
				SpellingError spellingError = GetSpellingError(CaretPosition);
				if (spellingError != null)
				{
					foreach (string suggestion in spellingError.Suggestions)
					{
						ContextMenu.Items.Insert(spellCheckMenuIndex++, new MenuItem()
						{
							Header = suggestion,
							FontWeight = FontWeights.Bold,
							Command = EditingCommands.CorrectSpellingError,
							CommandParameter = suggestion,
							CommandTarget = this,
						}
						);
					}

					if (spellCheckMenuIndex > 0)
					{
						ContextMenu.Items.Insert(spellCheckMenuIndex++, new Separator());

						ContextMenu.Items.Insert(spellCheckMenuIndex++, new MenuItem()
						{
							Header = @"Ignore All",
							Command = EditingCommands.IgnoreSpellingError,
							CommandTarget = this,
						}
						);

						ContextMenu.Items.Insert(spellCheckMenuIndex++, new Separator());
					}
				}
			}
		}

		private MenuItem CreateMenuEmoticons(string header, EmoticonDescription[] emoticons, int fromIndex, int toIndex)
		{
			var panelFactory = new FrameworkElementFactory(typeof(UniformGrid));
			panelFactory.SetValue(Control.BackgroundProperty, System.Windows.SystemColors.MenuBrush);

			var submenu = new MenuItem()
			{
				Header = header,
				ItemsPanel = new ItemsPanelTemplate(panelFactory),
				SnapsToDevicePixels = true,
			};

			for (int i = fromIndex; i < toIndex; i++)
			{
				submenu.Items.Add(
					new EmoticonMenuItem()
					{
						Icon = new AnimatedImage()
						{
							AnimatedBitmap = emoticons[i].Emoticon,
						},
						Command = RichTextBoxExCommands.InsertText,
						CommandParameter = emoticons[i].Smiles[0],
						CommandTarget = this,
						ToolTip = emoticons[i].Description + "\r\n" + emoticons[i].Smiles[0],
					}
				);
			}

			return submenu;
		}

		class EmoticonMenuItem : MenuItem
		{
			protected override Size MeasureOverride(Size availableSize)
			{
				if (Icon != null && Icon is AnimatedImage)
				{
					var size = (Icon as AnimatedImage).GetCurrentFrameSize();
					size.Width += Padding.Left + Padding.Right;
					size.Height += Padding.Top + Padding.Bottom;
					return size;
				}
				return new Size(0, 0);
			}
		}

		private static string GetHeader(RoutedUICommand command)
		{
			var header = new StringBuilder();
			foreach (var c in command.Name)
			{
				if (char.IsUpper(c))
					header.Append(' ');
				header.Append(c);
			}

			return header.ToString();
		}

		private MenuItem CreateFormatMenu()
		{
			var menu = new MenuItem() { Header = @"Style", };

			menu.Items.Add(new MenuItem()
			{
				FontWeight = FontWeights.Bold,
				Command = EditingCommands.ToggleBold,
				Header = GetHeader(EditingCommands.ToggleBold),
				InputGestureText = @"Ctrl+B",
			});

			menu.Items.Add(new MenuItem()
			{
				FontStyle = FontStyles.Italic,
				Command = EditingCommands.ToggleItalic,
				Header = GetHeader(EditingCommands.ToggleItalic),
				InputGestureText = @"Ctrl+I"
			});

			return menu;
		}

		private MenuItem CreateFontSizeMenu()
		{
			var menu = new MenuItem() { Header = @"Font Size", };

			menu.Items.Add(new MenuItem()
			{
				Command = EditingCommands.DecreaseFontSize,
				Header = GetHeader(EditingCommands.DecreaseFontSize),
				InputGestureText = @"Ctrl+["
			});

			menu.Items.Add(new MenuItem()
			{
				Command = EditingCommands.IncreaseFontSize,
				Header = GetHeader(EditingCommands.IncreaseFontSize),
				InputGestureText = @"Ctrl+]"
			});

			return menu;
		}

		private MenuItem CreateFontFamilyMenu()
		{
			var menu = new MenuItem() { Header = @"Font Family", };

			var fonts = new string[] { @"Arial", @"Arial Black", @"Courier New", @"Comic Sans MS", @"Impact", @"Verdana", @"Segoe UI", @"Times New Roman", @"Tahoma", };

			foreach (var font in fonts)
				menu.Items.Add(
					new MenuItem()
					{
						Header = font,
						FontFamily = new FontFamily(font),
						Command = RichTextBoxExCommands.SetFontFamily,
						CommandTarget = this,
						CommandParameter = font,
					});

			return menu;
		}

		private MenuItem CreateFontColorMenu(string header, RoutedUICommand command)
		{
			var panelFactory = new FrameworkElementFactory(typeof(UniformGrid));
			panelFactory.SetValue(Control.BackgroundProperty, System.Windows.SystemColors.MenuBrush);

			var menu = new MenuItem()
			{
				Header = header,
				ItemsPanel = new ItemsPanelTemplate(panelFactory),
				SnapsToDevicePixels = true,
			};

			#region var brushes = new Brush[] {...}

			var brushes = new Brush[] 
			{
				Brushes.AliceBlue,
				Brushes.AntiqueWhite,
				Brushes.Aqua,
				Brushes.Aquamarine,
				Brushes.Azure,
				Brushes.Beige,
				Brushes.Bisque,
				Brushes.Black,
				Brushes.BlanchedAlmond,
				Brushes.Blue,
				Brushes.BlueViolet,
				Brushes.Brown,
				Brushes.BurlyWood,
				Brushes.CadetBlue,
				Brushes.Chartreuse,
				Brushes.Chocolate,
				Brushes.Coral,
				Brushes.CornflowerBlue,
				Brushes.Cornsilk,
				Brushes.Crimson,
				Brushes.Cyan,
				Brushes.DarkBlue,
				Brushes.DarkCyan,
				Brushes.DarkGoldenrod,
				Brushes.DarkGray,
				Brushes.DarkGreen,
				Brushes.DarkKhaki,
				Brushes.DarkMagenta,
				Brushes.DarkOliveGreen,
				Brushes.DarkOrange,
				Brushes.DarkOrchid,
				Brushes.DarkRed,
				Brushes.DarkSalmon,
				Brushes.DarkSeaGreen,
				Brushes.DarkSlateBlue,
				Brushes.DarkSlateGray,
				Brushes.DarkTurquoise,
				Brushes.DarkViolet,
				Brushes.DeepPink,
				Brushes.DeepSkyBlue,
				Brushes.DimGray,
				Brushes.DodgerBlue,
				Brushes.Firebrick,
				Brushes.FloralWhite,
				Brushes.ForestGreen,
				Brushes.Fuchsia,
				Brushes.Gainsboro,
				Brushes.GhostWhite,
				Brushes.Gold,
				Brushes.Goldenrod,
				Brushes.Gray,
				Brushes.Green,
				Brushes.GreenYellow,
				Brushes.Honeydew,
				Brushes.HotPink,
				Brushes.IndianRed,
				Brushes.Indigo,
				Brushes.Ivory,
				Brushes.Khaki,
				Brushes.Lavender,
				Brushes.LavenderBlush,
				Brushes.LawnGreen,
				Brushes.LemonChiffon,
				Brushes.LightBlue,
				Brushes.LightCoral,
				Brushes.LightCyan,
				Brushes.LightGoldenrodYellow,
				Brushes.LightGray,
				Brushes.LightGreen,
				Brushes.LightPink,
				Brushes.LightSalmon,
				Brushes.LightSeaGreen,
				Brushes.LightSkyBlue,
				Brushes.LightSlateGray,
				Brushes.LightSteelBlue,
				Brushes.LightYellow,
				Brushes.Lime,
				Brushes.LimeGreen,
				Brushes.Linen,
				Brushes.Magenta,
				Brushes.Maroon,
				Brushes.MediumAquamarine,
				Brushes.MediumBlue,
				Brushes.MediumOrchid,
				Brushes.MediumPurple,
				Brushes.MediumSeaGreen,
				Brushes.MediumSlateBlue,
				Brushes.MediumSpringGreen,
				Brushes.MediumTurquoise,
				Brushes.MediumVioletRed,
				Brushes.MidnightBlue,
				Brushes.MintCream,
				Brushes.MistyRose,
				Brushes.Moccasin,
				Brushes.NavajoWhite,
				Brushes.Navy,
				Brushes.OldLace,
				Brushes.Olive,
				Brushes.OliveDrab,
				Brushes.Orange,
				Brushes.OrangeRed,
				Brushes.Orchid,
				Brushes.PaleGoldenrod,
				Brushes.PaleGreen,
				Brushes.PaleTurquoise,
				Brushes.PaleVioletRed,
				Brushes.PapayaWhip,
				Brushes.PeachPuff,
				Brushes.Peru,
				Brushes.Pink,
				Brushes.Plum,
				Brushes.PowderBlue,
				Brushes.Purple,
				Brushes.Red,
				Brushes.RosyBrown,
				Brushes.RoyalBlue,
				Brushes.SaddleBrown,
				Brushes.Salmon,
				Brushes.SandyBrown,
				Brushes.SeaGreen,
				Brushes.SeaShell,
				Brushes.Sienna,
				Brushes.Silver,
				Brushes.SkyBlue,
				Brushes.SlateBlue,
				Brushes.SlateGray,
				Brushes.Snow,
				Brushes.SpringGreen,
				Brushes.SteelBlue,
				Brushes.Tan,
				Brushes.Teal,
				Brushes.Thistle,
				Brushes.Tomato,
				Brushes.Transparent,
				Brushes.Turquoise,
				Brushes.Violet,
				Brushes.Wheat,
				Brushes.White,
				Brushes.WhiteSmoke,
				Brushes.Yellow,
				Brushes.YellowGreen,
			};

			#endregion

			foreach (var brush in brushes)
				menu.Items.Add(
					new ColorBoxMenuItem(brush)
					{
						Command = command,
						CommandTarget = this,
					});

			return menu;
		}

		class ColorBoxMenuItem : MenuItem
		{
			public ColorBoxMenuItem(Brush brush)
			{
				Padding = new Thickness(0.0);
				Margin = new Thickness(0.0);

				Icon = new System.Windows.Shapes.Rectangle()
				{
					Height = 10,
					Width = 10,
					Fill = brush,
					Stroke = Brushes.Black,
				};

				CommandParameter = brush;
			}

			protected override Size MeasureOverride(Size availableSize)
			{
				return new Size(24, 24);
			}
		}

		#endregion

		#region Emoticons: OnRenderSizeChanged, InitializeRabinKarp, AppendAllEmoticons, IsAutoEmoticoning

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			PresentationSource presentationSource = PresentationSource.FromVisual(this);
			if (presentationSource != null)
			{
				var newAnimationRect = TransformToAncestor(presentationSource.RootVisual).
					TransformBounds(new Rect(sizeInfo.NewSize));

				animationRect.Rect = newAnimationRect;
			}
		}

		private void InitializeRabinKarp()
		{
			rabinKarp = new RabinKarp<TextPointer, Bitmap>();

			foreach (var emoticon in YahooEmoticons.Emoticons)
			{
				foreach (var smile in emoticon.Smiles)
					rabinKarp.Add(smile, emoticon.Emoticon);
			}
		}

		public void AppendAllEmoticons()
		{
			foreach (var emoticon in YahooEmoticons.Emoticons)
			{
				this.AppendText(emoticon.Smiles[0]);
			}
		}

		protected bool IsAutoEmoticoning
		{
			get;
			set;
		}

		#endregion

		#region Emoticons: GetAnimatedBitmaps, ReplaceEmoticonWithSmile, ReplaceSmileWithEmoticon

		private IEnumerable<TextElement> GetAnimatedBitmaps(TextRange range)
		{
			List<TextElement> list = new List<TextElement>();

			for (TextPointer position = range.Start;
				position != null && position.CompareTo(range.End) <= 0;
				position = position.GetNextContextPosition(LogicalDirection.Forward))
			{
				if (position.GetPointerContext(LogicalDirection.Forward) ==
					TextPointerContext.ElementEnd)
				{
					if (position.Parent is InlineUIContainer)
					{
						if ((position.Parent as InlineUIContainer).Child is AnimatedImage)
							list.Add(position.Parent as TextElement);
					}
				}
			}

			return list;
		}

		private TextRange ReplaceEmoticonWithSmile(TextElement emoticon)
		{
			var selection = new TextRange(emoticon.ElementEnd, emoticon.ElementEnd);

			var smile = (((emoticon as InlineUIContainer).Child as AnimatedImage).Tag as string);

			if (string.IsNullOrEmpty(smile) == false)
			{
				selection.Text = smile;
				(new TextRange(emoticon.ElementStart, emoticon.ElementEnd)).Text = "";
			}
			else
			{
				selection = null;
			}

			return selection;
		}

		private void ReplaceSmileWithEmoticon(TextRange smile, Bitmap emoticon)
		{
			var tag = smile.Text;

			var inline = new InlineUIContainer(
				new AnimatedImage()
				{
					AnimatedBitmap = emoticon,
					Tag = tag,
					AnimationRect = animationRect,
				}
				, smile.Start);

			new TextRange(inline.ElementEnd, smile.End).Text = "";
		}

		#endregion

		#region Commands: InsertText, SetFontFamily, SetForeground, SetBackground, SetDefaultFont Commands

		private void Execute_InsertText(Object sender, ExecutedRoutedEventArgs e)
		{
			var range = new TextRange(CaretPosition, CaretPosition);
			range.Text = e.Parameter as string;

			CaretPosition = range.End;

			e.Handled = true;
		}

		private void CanExecute_InsertText(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsReadOnly;
			e.Handled = true;
		}

		private void Execute_SetFontFamily(Object sender, ExecutedRoutedEventArgs e)
		{
			Selection.ApplyPropertyValue(FontFamilyProperty, e.Parameter as string);
			e.Handled = true;
		}

		private void CanExecute_SetFontFamily(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsReadOnly;
			e.Handled = true;
		}

		private void Execute_SetForeground(Object sender, ExecutedRoutedEventArgs e)
		{
			Selection.ApplyPropertyValue(ForegroundProperty, e.Parameter as Brush);
			e.Handled = true;
		}

		private void CanExecute_SetForeground(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !IsReadOnly;
			e.Handled = true;
		}

		private void Execute_SetDefaultFont(Object sender, ExecutedRoutedEventArgs e)
		{
			Selection.ApplyPropertyValue(ForegroundProperty, Document.Foreground);
			Selection.ApplyPropertyValue(FontWeightProperty, Document.FontWeight);
			Selection.ApplyPropertyValue(FontStyleProperty, Document.FontStyle);
			Selection.ApplyPropertyValue(FontSizeProperty, Document.FontSize);
			Selection.ApplyPropertyValue(FontFamilyProperty, Document.FontFamily);

			e.Handled = true;
		}

		#endregion

		#region Commands: Copy, Cut

		private void Execute_Copy(Object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			Copy();
		}

		private void CanExecute_Copy(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !Selection.IsEmpty;
			e.Handled = true;
		}

		private void Execute_Cut(Object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			Cut();
		}

		private void CanExecute_Cut(Object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = !Selection.IsEmpty;
			e.Handled = true;
		}

		public new void Copy()
		{
			var selectionStart = Selection.Start;
			var selectionEnd = Selection.End;

			Clipboard.SetData(DataFormats.Rtf, GetRtf(Selection));

			Selection.Select(selectionStart, selectionEnd);
		}

		public new void Cut()
		{
			Clipboard.SetData(DataFormats.Rtf, CutRtf(Selection));
		}

		#endregion

		#region RTF

		private static double GetRtfFontSize(double fontSize)
		{
			var doc = new FlowDocument(new Paragraph(new Run("TEXT")))
			{
				FontSize = fontSize,
			};

			var rtf1 = GetRtfInternal(new TextRange(doc.ContentStart, doc.ContentEnd));
			var match = Regex.Match(rtf1, @"\\fs([1-9][0-9]*)");

			if (match != null && match.Success && match.Groups.Count > 1 && match.Groups[1].Success)
				return double.Parse(match.Groups[1].Value);

			return 0;
		}

		private static bool GetDefaultFont(string rtf, out string family, out int size)
		{
			var match = Regex.Match(rtf, @"^{\\rtf1{\\\*\\officesip{\\xfont (?<family>[^;]+);}\\xsize(?<size>[0-9]+)}");

			if (match.Success)
			{
				family = match.Groups[@"family"].Value;
				size = int.Parse(match.Groups[@"size"].Value);
			}
			else
			{
				family = null;
				size = 0;
			}

			return match.Success;
		}

		private static string ReplaceDefaultFont(string rtf, string oldFamily, int oldSize, string newFamily, int newSize)
		{
			string fontId = null;

			rtf = Regex.Replace(rtf, @"{\\f(?<id>[0-9]+)\\fcharset[0-9]+ " + oldFamily + ";}",
				new MatchEvaluator(
					(match) =>
					{
						fontId = match.Groups["id"].Value;
						return match.Value.Replace(oldFamily, newFamily);
					}
				)
			);

			if (fontId != null)
			{
				rtf = Regex.Replace(rtf, @"\\f" + fontId + @"(\\[a-z]{1,2}){0,3}\\fs" + oldSize,
					new MatchEvaluator(
						(match) =>
						{
							return match.Value.Remove(match.Value.Length - oldSize.ToString().Length) + newSize;
						}
					)
				);
				rtf = Regex.Replace(rtf, @"\\fs" + oldSize + @"(\\[a-z]{1,2}){0,3}\\f" + fontId,
					new MatchEvaluator(
						(match) =>
						{
							return @"\fs" + newSize + match.Value.Substring(3 + oldSize.ToString().Length);
						}
					)
				);
			}

			return rtf;
		}

		private static string GetRtfInternal(TextRange range)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				range.Save(memoryStream, DataFormats.Rtf);

				memoryStream.Flush();
				memoryStream.Seek(0, SeekOrigin.Begin);

				using (StreamReader reader = new StreamReader(memoryStream))
					return reader.ReadToEnd();
			}
		}

		private static void SetRtfInternal(TextRange range, string rtf)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			using (StreamWriter writer = new StreamWriter(memoryStream))
			{
				writer.Write(rtf);
				writer.Flush();

				memoryStream.Seek(0, SeekOrigin.Begin);

				range.Load(memoryStream, DataFormats.Rtf);
			}
		}

		public string InsertDefaultFontInfo(string rtf)
		{
			return rtf.StartsWith(@"{\rtf1") ?
				rtf.Insert(6, @"{\*\officesip{\xfont " + Document.FontFamily + @";}" + @"\xsize" + (int)GetRtfFontSize(FontSize) + @"}") : rtf;
		}

		private string GetRtf(TextRange range, bool cut)
		{
			IsUndoEnabled = IsAutoEmoticoning = false;

			var emoticons = GetAnimatedBitmaps(range);

			var smiles = cut ? null : new List<TextRange>();
			foreach (var emoticon in emoticons)
			{
				var smile = ReplaceEmoticonWithSmile(emoticon);
				if (smile != null && smiles != null)
					smiles.Add(smile);
			}

			string rtf = GetRtfInternal(range);

			if (cut)
				range.Text = @"";

			if (smiles != null)
				foreach (var smile in smiles)
				{
					var emoticon = rabinKarp.Get(smile.Text);
					if (emoticon != null)
						ReplaceSmileWithEmoticon(smile, emoticon);
				}

			IsUndoEnabled = IsAutoEmoticoning = true;

			return rtf;
		}

		public string CutRtf(TextRange range)
		{
			return GetRtf(range, true);
		}

		public string GetRtf(TextRange range)
		{
			return GetRtf(range, false);
		}

		public void SetRtf(TextRange range, string rtf)
		{
			string family;
			int size;

			if (GetDefaultFont(rtf, out family, out size))
				rtf = ReplaceDefaultFont(rtf, family, size, FontFamily.ToString(), (int)GetRtfFontSize(FontSize));

			SetRtfInternal(range, rtf);
		}

		public void AppendRtf(string rtf)
		{
			SetRtf(new TextRange(Document.ContentEnd, Document.ContentEnd), rtf);
		}

		#endregion

		#region OnTextChanged

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			if (IsAutoEmoticoning)
			{
				IsAutoEmoticoning = false;

				rabinKarp.Reset();
				var smiles = new List<EmoticonSmile>();

				TextPointer smileStart, smileEnd;
				Bitmap image;
				TextPointer end = null;

				foreach (var change in e.Changes)
				{
					if (change.AddedLength > 0)
					{
						var start = Document.ContentStart.GetPositionAtOffset(
							(change.Offset > rabinKarp.MaxLength) ? change.Offset - rabinKarp.MaxLength : 0);

						while (end != null && start != null && start.CompareTo(end) <= 0)
							start = start.GetNextContextPosition(LogicalDirection.Forward);
						if (start == null)
							break; // end of document - stop

						end = Document.ContentStart.GetPositionAtOffset(change.Offset + change.AddedLength + rabinKarp.MaxLength);
						if (end == null)
							end = Document.ContentEnd;


						for (var current = start; current != null && current.CompareTo(end) <= 0;
										current = current.GetNextContextPosition(LogicalDirection.Forward))
						{
							if (current.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
							{
								string text = current.GetTextInRun(LogicalDirection.Forward);

								for (int i = 0; i < text.Length; i++)
								{
									if (rabinKarp.Step(text[i], current.GetPositionAtOffset(i), current.GetPositionAtOffset(i + 1), out image, out smileStart, out smileEnd))
									{
										var smile = new EmoticonSmile(smileStart, smileEnd, image);
										smiles.Add(smile);
									}
								}
							}

							if (current.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
							{
								if (current.Parent is InlineUIContainer)
								{
									if ((current.Parent as InlineUIContainer).Child is AnimatedImage)
									{
										var animatedBitmap = ((current.Parent as InlineUIContainer).Child as AnimatedImage);
										if (animatedBitmap.AnimatedBitmap == null)
										{
											var tag = animatedBitmap.Tag as string;
											if (string.IsNullOrEmpty(tag) == false)
												animatedBitmap.AnimatedBitmap = rabinKarp.Get(tag);
										}
									}
								}
							}
						}

						for (int i = 0; i < rabinKarp.EndSteps; i++)
						{
							if (rabinKarp.Step(out image, out smileStart, out smileEnd))
								smiles.Add(new EmoticonSmile(smileStart, smileEnd, image));
						}
					}
				}

				foreach (var smile in smiles)
					ReplaceSmileWithEmoticon(smile.TextRange, smile.Emoticon);

				IsAutoEmoticoning = true;
			}
		}

		class EmoticonSmile
		{
			public EmoticonSmile(TextPointer start, TextPointer end, Bitmap emoticon)
			{
				TextRange = new TextRange(start, end);
				Emoticon = emoticon;
			}

			public readonly TextRange TextRange;
			public readonly Bitmap Emoticon;
		}

		/*
							string smileBefore = "";

							if (current.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
							{
								if (current.Parent is InlineUIContainer)
								{
									if ((current.Parent as InlineUIContainer).Child is AnimatedImage)
									{
										smileBefore = (((current.Parent as InlineUIContainer).Child as AnimatedImage).Tag as string);
									}
								}
							}
		*/
		#endregion

		#region Drag-and-Drop

		private TextPointer dragStart, dragEnd;

		private void NoDragCopy(object sender, DataObjectCopyingEventArgs e)
		{
			if (e.IsDragDrop)
			{
				dragStart = Selection.Start;
				dragEnd = Selection.End;

				e.CancelCommand();

				Dispatcher.BeginInvoke(
					System.Windows.Threading.DispatcherPriority.Normal, new EventHandler(DoDragDrop),
					this, new EventArgs());
			}
		}

		private void DoDragDrop(object sender, EventArgs e)
		{
			Selection.Select(dragStart, dragEnd);

			DataObject data = new DataObject(System.Windows.DataFormats.Rtf.ToString(), GetRtf(new TextRange(dragStart, dragEnd)));
			DragDropEffects ddEffects = DragDrop.DoDragDrop(new TextBox(), data, DragDropEffects.Move | DragDropEffects.Copy);

			if (ddEffects == DragDropEffects.Move && Selection.Start.CompareTo(dragStart) == 0)
				Selection.Text = @"";
		}

		//protected override void OnDragEnter(System.Windows.DragEventArgs e)
		//{
		//}

		//protected override void OnDragLeave(System.Windows.DragEventArgs e)
		//{
		//}

		//protected override void OnDragOver(System.Windows.DragEventArgs e)
		//{
		//}

		//protected override void OnDrop(System.Windows.DragEventArgs e)
		//{
		//}

		//protected override void OnGiveFeedback(System.Windows.GiveFeedbackEventArgs e)
		//{
		//}

		//bool isDragging;
		//Point dragStartPoint;

		//protected override void OnPreviewMouseMove(MouseEventArgs e)
		//{
		//    //	e.Handled = true;
		//}

		//protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		//{
		//    //	e.Handled = true;
		//}

		//void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
		//{
		//    if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
		//    {
		//        Point position = e.GetPosition(null);

		//        if (Math.Abs(position.X - dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
		//            Math.Abs(position.Y - dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
		//        {
		//            //StartDrag(e);
		//        }
		//    }
		//}

		//void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		//{
		//    dragStartPoint = e.GetPosition(null);
		//}

		#endregion

		#region Helpers: RemoveEmptyLastParagraph

		public void InsertNewLine()
		{
			Selection.Text = Environment.NewLine;
			CaretPosition = Selection.End;
			Selection.Select(CaretPosition, CaretPosition);
		}

		public void RemoveEmptyLastParagraph()
		{
			if (Document.Blocks.LastBlock is Paragraph)
			{
				var paragraph = Document.Blocks.LastBlock as Paragraph;

				if (paragraph.Inlines.Count == 0)
					Document.Blocks.Remove(paragraph);
				else if (paragraph.Inlines.Count == 1)
				{
					if (paragraph.Inlines.FirstInline is Run)
					{
						var run = paragraph.Inlines.FirstInline as Run;
						if (run.Text.Length == 0)
							Document.Blocks.Remove(paragraph);
					}
				}
			}
		}

		#endregion
	}
}

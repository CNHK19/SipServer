// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Messenger.Windows
{
	public class BaseCommands
	{
		public static RoutedUICommand CreateRoutedUICommand(string name, Type type)
		{
			return CreateRoutedUICommand(name, type, null);
		}

		public static RoutedUICommand CreateRoutedUICommand(string name, Type type, InputGestureCollection gestures)
		{
			string text = LoadString(name);

			if (gestures == null)
				return new RoutedUICommand(text, name, type);
			else
				return new RoutedUICommand(text, name, type, gestures);
		}

		public static string LoadString(string name)
		{
			string text = null;

			if (Application.Current != null)
				text = Application.Current.Resources[name] as string;

			if (text == null)
				text = AddSpace(name);

			return text;
		}

		public static string AddSpace(string source)
		{
			var result = string.Empty;

			foreach (var c in source)
			{
				if (char.IsUpper(c))
					result += ' ';
				result += c;
			}

			return result;
		}

		public static InputGestureCollection CreateGesture(Key key, ModifierKeys modifiers)
		{
			var gestureCollection = new InputGestureCollection();
			gestureCollection.Add(new KeyGesture(key, modifiers));

			return gestureCollection;
		}
	}
}
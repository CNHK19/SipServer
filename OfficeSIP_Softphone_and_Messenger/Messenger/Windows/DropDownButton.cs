// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Messenger.Windows
{
	/// <summary>
	/// From: http://andyonwpf.blogspot.com/2006/10/dropdownbuttons-in-wpf.html
	/// </summary>
	public class DropDownButton : ToggleButton
	{
		public static readonly DependencyProperty DropDownProperty = DependencyProperty.Register("DropDown", typeof(ContextMenu), typeof(DropDownButton), new UIPropertyMetadata(null));

		public DropDownButton()
		{
			Binding binding = new Binding("DropDown.IsOpen");
			binding.Source = this;
			this.SetBinding(IsCheckedProperty, binding);
		}

		public ContextMenu DropDown
		{
			get
			{
				return (ContextMenu)GetValue(DropDownProperty);
			}
			set
			{
				SetValue(DropDownProperty, value);
			}
		}

		protected override void OnClick()
		{
			if (DropDown != null)
			{
				DropDown.PlacementTarget = this;
				DropDown.Placement = PlacementMode.Bottom;

				DropDown.IsOpen = true;
			}
		}
	}
}

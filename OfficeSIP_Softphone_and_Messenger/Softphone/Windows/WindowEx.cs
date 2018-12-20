// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;

namespace Messenger.Windows
{
	public class WindowEx
		: Window
		, INotifyPropertyChanged
	{
		public WindowEx()
		{
#if DEBUG
			if (DesignerProperties.GetIsInDesignMode(this) == false)
#endif
				CommandBindings.AddRange(
					Application.Current.FindResource(@"GlobalCommands") as CommandBindingCollection);

			DataContext = this;
		}

		public bool Result
		{
			get;
			protected set;
		}

		#region InvalidateRequerySuggested

		private delegate void Delegate1();

		private Delegate1 invalidateRequerySuggestedDelegate;

		protected void Dispatcher_BeginInvoke_InvalidateRequerySuggested()
		{
			if (invalidateRequerySuggestedDelegate == null)
				invalidateRequerySuggestedDelegate = new Delegate1(CommandManager.InvalidateRequerySuggested);

			Dispatcher.BeginInvoke(invalidateRequerySuggestedDelegate, null);
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
	}
}

// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace Uccapi
{
	public class UccapiBase
		: INotifyPropertyChanged
	{
		private static Dispatcher dispatcher;

		public UccapiBase()
		{
		}

		public UccapiBase(Dispatcher dispatcher1)
		{
			dispatcher = dispatcher1;
		}

		#region OnEvent<...>

		protected void OnEvent<TEventArgs>(EventHandler<TEventArgs> handler, TEventArgs args)
			where TEventArgs : EventArgs
		{
			if (handler != null)
			{
				if (dispatcher != null)
					dispatcher.BeginInvoke(handler, this, args);
				else
					handler(this, args);
			}
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(String property)
		{
			try
			{
				OnPropertyChanged(new PropertyChangedEventArgs(Helpers.StriptPropertyName(property)));
			}
			catch { }
		}

		protected void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
		{
			if (PropertyChanged != null)
			{
				if (dispatcher != null)
					dispatcher.Invoke(PropertyChanged, this, eventArgs);
				else
					PropertyChanged(this, eventArgs);
			}
		}		

		#endregion INotifyPropertyChanged
	}
}
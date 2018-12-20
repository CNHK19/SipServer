// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Messenger
{
	public interface IProgrammeData
		: INotifyPropertyChanged
	{
		ObservableCollection<string> OutgoingCalls { get; }
		string CallError { get; }
	}

	partial class Programme
		: IProgrammeData
	{
		public ObservableCollection<string> OutgoingCalls 
		{
			get { return outgoingCalls; }
		}

		#region CallError

		private string callError;

		public string CallError
		{
			get { return callError; }
			set
			{
				if (callError != value)
				{
					callError = value;
					OnPropertyChanged(@"CallError");
				}
			}
		}

		#endregion

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(String property)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(property));
		}

		protected void OnPropertyChanged(PropertyChangedEventArgs eventArgs)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, eventArgs);
		}

		#endregion INotifyPropertyChanged
	}
}
// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Uccapi
{
	public interface IPresentitiesCollection
		: IList<IPresentity>
		, ICollection<IPresentity>
		, IEnumerable<IPresentity>
		, IList
		, ICollection
		, IEnumerable
		, INotifyCollectionChanged 
		, INotifyPropertyChanged
	{
		event PropertyChangedEventHandler ItemPropertyChanged;

		ObservableCollection<string> Groups { get; }

		bool Contain(string uri);

		new void Clear();
		
		void AddRange(ICollection<IPresentity> collection);
		void RemoveRange(ICollection<IPresentity> collection);
		void RemoveRange(IList list);
	}
}

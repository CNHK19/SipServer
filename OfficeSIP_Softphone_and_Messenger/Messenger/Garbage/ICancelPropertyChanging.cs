// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;

namespace Garbage.Uccapi
{
	public delegate void CancelPropertyChangingEventHandler(Object sender, CancelPropertyChangingEventArgs e);

	interface ICancelPropertyChanging
	{
		event CancelPropertyChangingEventHandler CancelPropertyChanging;
	}

	public class CancelPropertyChangingEventArgs
		: CancelEventArgs
	{
		public CancelPropertyChangingEventArgs(string propertyName)
		{
			this.PropertyName = propertyName;
		}

		public string PropertyName { get; private set; }
	}

		//#region ICancelPropertyChanging

		//public event CancelPropertyChangingEventHandler CancelPropertyChanging;

		//private bool OnCancelPropertyChanging(string property)
		//{
		//    if (this.CancelPropertyChanging != null)
		//    {
		//        CancelPropertyChangingEventArgs eventArgs = new CancelPropertyChangingEventArgs(property);
		//        CancelPropertyChanging(this, eventArgs);
		//        return eventArgs.Cancel;
		//    }
		//    return false;
		//}

		//#endregion
}

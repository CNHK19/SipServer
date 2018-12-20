// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Uccapi;

namespace Messenger.Windows
{
	class AvChatController
	{
		public void Sessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (Session session in e.NewItems)
					if (session is IAvSession)
					{
						AvChat avChat = new AvChat(session as IAvSession);
						avChat.Show();
					}
			}
		}
	}
}

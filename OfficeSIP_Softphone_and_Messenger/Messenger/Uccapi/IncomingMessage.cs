// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;

namespace Uccapi
{
	class IncomingMessage
		: IIncomingMessage
	{
		public string FromUri { get; set; }
		public string ContentType { get; set; }
		public string Message { get; set; }
		public DateTime DateTime { get; set; }
	}
}

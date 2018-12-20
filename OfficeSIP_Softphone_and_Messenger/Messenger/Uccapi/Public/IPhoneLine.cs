// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uccapi
{
	public enum PhoneLineType
	{
		Dual,
		None,
		RemoteCallControl,
		UnifiedCommunications
	}

	public interface IPhoneLine
	{
		string Uri { get; }
		PhoneLineType LineType { get; }
		string LineServer { get; }
	}
}

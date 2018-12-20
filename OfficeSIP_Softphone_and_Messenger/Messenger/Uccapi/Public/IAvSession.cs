// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
	public enum MediaDirection
	{
		Send = UCC_MEDIA_DIRECTIONS.UCCMD_SEND,
		Receive = UCC_MEDIA_DIRECTIONS.UCCMD_RECEIVE,
	}

	public interface IAvSession
		: ISession
	{
		VideoWindowHost VideoWindow { get; }

		bool SendDtmf(char dtmf);

		void EnableVideo();
		int VideoInChannelCount { get; }
		int VideoOutChannelCount { get; }
		int AudioInChannelCount { get; }
		int AudioOutChannelCount { get; }
	}
}

// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Uccapi
{
	public interface ISession
		: INotifyPropertyChanged
	{
		SessionType SessionType { get; }
		bool IsTerminated();
		ParticipantLog AddPartipant(string uri);
		void Destroy();
		ObservableCollection<ParticipantLog> PartipantLogs { get; }
		ObservableCollection<ParticipantLog> ParticipantLogs { get; }
		bool HasRemoteConnectedParticipants();
		bool HasConnectingParticipants();
		ParticipantLog FindParticipantLog(string uri);
		string GetDisplayNameOrAor(string uri);
		IPresentity SelfPresentity { get; }
	}
}

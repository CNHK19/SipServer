// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
	public enum TransportMode
	{
		Udp = 0,
		Tcp,
		Tls
	}

	public class SignalingServer
	{
		public string ServerAddress { get; set; }
		public TransportMode TransportMode { get; set; }
	}

	public class EndpointConfiguration
	{
		public string Uri { get; set; }
		public bool DetectServer { get; set; }
		public SignalingServer SignalingServer { get; set; }

		#region public int EndpoindId

		public int EndpointId { get; set; }

		public string EndpoindIdString
		{
			get
			{
				if (this.EndpointId <= 0)
					return null;
				return this.EndpointId.ToString();
			}
		}

		#endregion

		public AuthenticationMode[] AuthenticationModes { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public string Realm { get; set; }
		public int AvInviteTimeout { get; set; }

		public static readonly AuthenticationMode[] AllAuthenticationModes;
		public static readonly AuthenticationMode[] DefaultAuthenticationModes;
		public static readonly AuthenticationMode[] CustomAuthenticationModes;

		public bool DisableImSessions { get; set; }
		public bool DisablePublicationsSubscriptions { get; set; }

		public EndpointConfiguration()
		{
			AvInviteTimeout = 60;
		}

		static EndpointConfiguration()
		{
			var modes = Enum.GetValues(typeof(AuthenticationMode));

			int defaulLength = 0;
			foreach (AuthenticationMode mode in modes)
				if (mode.IsDefaultCreditals())
					defaulLength++;

			AllAuthenticationModes = new AuthenticationMode[modes.Length];
			DefaultAuthenticationModes = new AuthenticationMode[defaulLength];
			CustomAuthenticationModes = new AuthenticationMode[modes.Length - defaulLength];

			int defaultCount = 0, customCount = 0;
			foreach (AuthenticationMode mode in modes)
			{
				AllAuthenticationModes[defaultCount + customCount] = mode;
				if (mode.IsDefaultCreditals())
					DefaultAuthenticationModes[defaultCount++] = mode;
				else
					CustomAuthenticationModes[customCount++] = mode;
			}
		}
	}
}

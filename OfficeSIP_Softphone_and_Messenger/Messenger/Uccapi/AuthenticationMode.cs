// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
	public enum AuthenticationMode
	{
		KerberosNtlmDefaultCreditals,
		KerberosDefaultCreditals,
		NtlmDefaultCreditals,
		NtlmDigestCustomCreditals,
		NtlmCustomCreditals,
		DigestCustomCreditals,
	}

	public static class AuthenticationModeConverter
	{
		public static UCC_AUTHENTICATION_MODES ToUccAuthenticationMode(this AuthenticationMode authMode)
		{
			switch (authMode)
			{
				case AuthenticationMode.KerberosDefaultCreditals:
					return UCC_AUTHENTICATION_MODES.UCCAM_KERBEROS;

				case AuthenticationMode.KerberosNtlmDefaultCreditals:
					return UCC_AUTHENTICATION_MODES.UCCAM_KERBEROS | UCC_AUTHENTICATION_MODES.UCCAM_NTLM;

				case AuthenticationMode.NtlmDefaultCreditals:
				case AuthenticationMode.NtlmCustomCreditals:
					return UCC_AUTHENTICATION_MODES.UCCAM_NTLM;

				case AuthenticationMode.NtlmDigestCustomCreditals:
					return UCC_AUTHENTICATION_MODES.UCCAM_NTLM | UCC_AUTHENTICATION_MODES.UCCAM_DIGEST;

				case AuthenticationMode.DigestCustomCreditals:
					return UCC_AUTHENTICATION_MODES.UCCAM_DIGEST;
			}

			throw new Exception(@"AuthenticationMode");
		}

		public static bool IsDefaultCreditals(this AuthenticationMode authMode)
		{
			switch (authMode)
			{
				case AuthenticationMode.KerberosDefaultCreditals:
				case AuthenticationMode.KerberosNtlmDefaultCreditals:
				case AuthenticationMode.NtlmDefaultCreditals:
					return true;

				case AuthenticationMode.NtlmCustomCreditals:
				case AuthenticationMode.NtlmDigestCustomCreditals:
				case AuthenticationMode.DigestCustomCreditals:
					return false;
			}

			throw new Exception(@"AuthenticationMode");
		}
	}
}
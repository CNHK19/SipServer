// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.UccApi;

namespace Uccapi
{
    public static class Helpers
    {
        private const string SipPrefix = @"sip:";
        private const string SipsPrefix = @"sips:";

        private static Object syncUriManager = new Object();
        private static UccUriManager uriManager = new UccUriManagerClass();

        public static UccUri ParseUri(string uri)
        {
            lock (syncUriManager)
            {
                return uriManager.ParseUri(uri);
            }
        }

        public static bool TryParseUri(string uri, out UccUri uccUri)
        {
            lock (syncUriManager)
            {
                uccUri = null;

                try
                {
                    if (string.IsNullOrEmpty(uri) == false)
                        uccUri = uriManager.ParseUri(uri);
                }
                catch (COMException)
                {
                }

                return uccUri != null;
            }
        }

        public static bool IsInvalidUri(string uri)
        {
            lock (syncUriManager)
            {
                try
                {
                    if (uri == null || uri.Length == 0)
                        return true;

                    if (uri.Contains(@"@") == false)
                        return true;

                    uriManager.ParseUri(uri);
                }
                catch (COMException)
                {
                    return true;
                }

                return false;
            }
        }

        public static string CorrectUri(string uri)
        {
            if (uri == null || uri.Length == 0)
                return uri;

            if (uri.IndexOf(SipPrefix) == 0)
                return uri;
            if (uri.IndexOf(SipsPrefix) == 0)
                return uri;
            return SipPrefix + uri;
        }

        public static string NakeUri(string uri)
        {
            return Helpers.GetAor(uri);
        }

        public static string GetAor(string uri)
        {
            if (string.IsNullOrEmpty(uri) == false)
            {
                if (uri.IndexOf(SipsPrefix) == 0)
                    return uri.Substring(SipsPrefix.Length);
                if (uri.IndexOf(SipPrefix) == 0)
                    return uri.Substring(SipPrefix.Length);
            }
            return uri;
        }

        public static string GetDomain(string uri)
        {
            UccUri uccUri;
            if (TryParseUri(CorrectUri(uri), out uccUri))
                return uccUri.Host;

            return null;
        }

        public static bool IsOperationCompleteOk(IUccOperationProgressEvent eventData)
        {
            return eventData.IsComplete && eventData.StatusCode >= 0;
        }

        public static bool IsOperationCompleteFailed(IUccOperationProgressEvent eventData)
        {
            return eventData.IsComplete && eventData.StatusCode < 0;
        }

        public static bool IsOperationSuccess(int statusCode)
        {
            return statusCode >= 0;
        }

        public static bool IsOperationFailed(int statusCode)
        {
            return statusCode < 0;
        }

        public static bool IsUriEqual(string uri1, string uri2)
        {
            return String.Compare(CorrectUri(uri1), CorrectUri(uri2), true) == 0;
        }

        public static bool IsUriEqual(string uri1, UccUri uri2)
        {
            return Helpers.IsUriEqual(uri1, uri2.Value);
        }

        public static string StriptPropertyName(string propertyName)
        {
            if (propertyName.StartsWith(@"set_"))
                return propertyName.Substring(4);
            return propertyName;
        }

        public static TransportMode Convert(UCC_TRANSPORT_MODE ucc)
        {
            switch (ucc)
            {
                case UCC_TRANSPORT_MODE.UCCTM_UDP:
                    return TransportMode.Udp;
                case UCC_TRANSPORT_MODE.UCCTM_TCP:
                    return TransportMode.Tcp;
                case UCC_TRANSPORT_MODE.UCCTM_TLS:
                    return TransportMode.Tls;
            }

            throw new NotSupportedException("Not supported transport mode");
        }

        public static UCC_TRANSPORT_MODE Convert(TransportMode ucc)
        {
            switch (ucc)
            {
                case TransportMode.Udp:
                    return UCC_TRANSPORT_MODE.UCCTM_UDP;
                case TransportMode.Tcp:
                    return UCC_TRANSPORT_MODE.UCCTM_TCP;
                case TransportMode.Tls:
                    return UCC_TRANSPORT_MODE.UCCTM_TLS;
            }

            throw new NotSupportedException("Not supported transport mode");
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception)
            {
                return "";
            }

        }

        public static bool CheckFileAvailability(string fileName, bool CheckForWriting)
        {
            try
            {
                FileStream file;
                if (CheckForWriting)
                {
                    file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                    file.WriteByte(255); 
                    file.Close();
                }
                else
                {
                    file = new FileStream(fileName, FileMode.Open);
                    file.ReadByte();
                    file.Close();
                }

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}

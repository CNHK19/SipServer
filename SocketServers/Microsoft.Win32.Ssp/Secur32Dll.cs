using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.Ssp
{
	[SuppressUnmanagedCodeSecurity]
	internal static class Secur32Dll
	{
		private const string SECUR32 = "secur32.dll";

		public const string UNISP_NAME = "Microsoft Unified Security Protocol Provider";

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int FreeContextBuffer([In] IntPtr pvContextBuffer);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int FreeCredentialsHandle([In] ref CredHandle phCredential);

		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int EnumerateSecurityPackagesA(out int pcPackages, out SafeContextBufferHandle ppPackageInfo);

		[DllImport("secur32.dll", BestFitMapping = false, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true, ThrowOnUnmappableChar = true)]
		public unsafe static extern int AcquireCredentialsHandleA([MarshalAs(UnmanagedType.LPStr)] [In] string pszPrincipal, [MarshalAs(UnmanagedType.LPStr)] [In] string pszPackage, [In] int fCredentialUse, [In] void* pvLogonID, [In] void* pAuthData, [In] void* pGetKeyFn, [In] void* pvGetKeyArgument, out CredHandle phCredential, out long ptsExpiry);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public unsafe static extern int AcceptSecurityContext([In] ref CredHandle phCredential, [In] [Out] void* phContext, [In] ref SecBufferDesc pInput, [In] int fContextReq, [In] int TargetDataRep, [In] [Out] ref CtxtHandle phNewContext, [In] [Out] ref SecBufferDesc pOutput, out int pfContextAttr, out long ptsTimeStamp);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int DeleteSecurityContext([In] ref CtxtHandle phContext);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public unsafe static extern int QueryContextAttributesA([In] ref CtxtHandle phContext, [In] uint ulAttribute, [Out] void* pBuffer);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int MakeSignature([In] ref CtxtHandle phContext, [In] int fQOP, [In] [Out] ref SecBufferDesc pMessage, [In] int MessageSeqNo);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int VerifySignature([In] ref CtxtHandle phContext, [In] ref SecBufferDesc pMessage, [In] int MessageSeqNo, out int pfQOP);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern int DecryptMessage([In] ref CtxtHandle phContext, [In] [Out] ref SecBufferDesc pMessage, [In] uint MessageSeqNo, out uint pfQOP);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public unsafe static extern int DecryptMessage([In] ref CtxtHandle phContext, [In] [Out] ref SecBufferDesc pMessage, [In] uint MessageSeqNo, [Out] void* pfQOP);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("secur32.dll", ExactSpelling = true, SetLastError = true)]
		public unsafe static extern int EncryptMessage([In] ref CtxtHandle phContext, [Out] void* pfQOP, [In] [Out] ref SecBufferDesc pMessage, [In] uint MessageSeqNo);
	}
}

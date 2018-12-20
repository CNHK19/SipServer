using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.Ssp
{
	public static class Sspi
	{
		public static bool Succeeded(SecurityStatus result)
		{
			return result >= SecurityStatus.SEC_E_OK;
		}

		public static bool Failed(SecurityStatus result)
		{
			return result < SecurityStatus.SEC_E_OK;
		}

		internal static bool Succeeded(int result)
		{
			return result >= 0;
		}

		internal static bool Failed(int result)
		{
			return result < 0;
		}

		public static SecurityStatus EnumerateSecurityPackages(out int packages, out SafeContextBufferHandle secPkgInfos)
		{
			return Sspi.Convert(Secur32Dll.EnumerateSecurityPackagesA(out packages, out secPkgInfos));
		}

		public unsafe static void AcquireCredentialsHandle(CredentialUse credentialUse, SchannelCred authData, out SafeCredHandle credential, out long expiry)
		{
			GCHandle gCHandle = default(GCHandle);
			IntPtr[] array = null;
			if (authData.cCreds > 0)
			{
				array = new IntPtr[]
				{
					authData.paCreds1
				};
				gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
				authData.paCreds1 = gCHandle.AddrOfPinnedObject();
			}
			try
			{
				CredHandle credHandle;
				int num = Secur32Dll.AcquireCredentialsHandleA(null, "Microsoft Unified Security Protocol Provider", (int)credentialUse, null, (void*)(&authData), null, null, out credHandle, out expiry);
				if (num != 0)
				{
					throw new SspiException(num, "AcquireCredentialsHandleA");
				}
				credential = new SafeCredHandle(credHandle);
			}
			finally
			{
				if (gCHandle.IsAllocated)
				{
					gCHandle.Free();
				}
				if (array != null)
				{
					authData.paCreds1 = array[0];
				}
			}
		}

		public static SafeCredHandle SafeAcquireCredentialsHandle(string package, CredentialUse credentialUse)
		{
			CredHandle credHandle;
			long num;
			Secur32Dll.AcquireCredentialsHandleA(null, package, (int)credentialUse, null, null, null, null, out credHandle, out num);
			return new SafeCredHandle(credHandle);
		}

		public static SecurityStatus SafeAcceptSecurityContext(ref SafeCredHandle credential, ref SafeCtxtHandle context, ref SecBufferDescEx input, int contextReq, TargetDataRep targetDataRep, ref SafeCtxtHandle newContext, ref SecBufferDescEx output)
		{
			int num;
			long num2;
			return Sspi.SafeAcceptSecurityContext(ref credential, ref context, ref input, contextReq, targetDataRep, ref newContext, ref output, out num, out num2);
		}

		public unsafe static SecurityStatus SafeAcceptSecurityContext(ref SafeCredHandle credential, ref SafeCtxtHandle context, ref SecBufferDescEx input, int contextReq, TargetDataRep targetDataRep, ref SafeCtxtHandle newContext, ref SecBufferDescEx output, out int contextAttr, out long timeStamp)
		{
			SecurityStatus result;
			try
			{
				input.Pin();
				output.Pin();
				try
				{
					fixed (IntPtr* ptr = (IntPtr*)(&context.Handle))
					{
						int error = Secur32Dll.AcceptSecurityContext(ref credential.Handle, context.IsInvalid ? null : ((void*)ptr), ref input.SecBufferDesc, contextReq, (int)targetDataRep, ref newContext.Handle, ref output.SecBufferDesc, out contextAttr, out timeStamp);
						result = Sspi.Convert(error);
					}
				}
				finally
				{
					IntPtr* ptr = null;
				}
			}
			catch
			{
				contextAttr = 0;
				timeStamp = 0L;
				result = (SecurityStatus)4294967295u;
			}
			finally
			{
				input.Free();
				output.Free();
			}
			return result;
		}

		public unsafe static SecurityStatus SafeDecryptMessage(ref SafeCtxtHandle context, ref SecBufferDescEx message, uint MessageSeqNo, void* pfQOP)
		{
			SecurityStatus result;
			try
			{
				message.Pin();
				int error = Secur32Dll.DecryptMessage(ref context.Handle, ref message.SecBufferDesc, MessageSeqNo, pfQOP);
				result = Sspi.Convert(error);
			}
			catch
			{
				result = (SecurityStatus)4294967295u;
			}
			finally
			{
				message.Free();
			}
			return result;
		}

		public unsafe static void EncryptMessage(ref SafeCtxtHandle context, ref SecBufferDescEx message, uint MessageSeqNo, void* pfQOP)
		{
			try
			{
				message.Pin();
				int num = Secur32Dll.EncryptMessage(ref context.Handle, pfQOP, ref message.SecBufferDesc, MessageSeqNo);
				if (num != 0)
				{
					throw new SspiException(num, "EncryptMessage");
				}
			}
			finally
			{
				message.Free();
			}
		}

		public unsafe static void QueryContextAttributes(ref SafeCtxtHandle context, out SecPkgContext_StreamSizes streamSizes)
		{
			fixed (IntPtr* ptr = (IntPtr*)(&streamSizes))
			{
				Sspi.QueryContextAttributes(ref context, UlAttribute.SECPKG_ATTR_STREAM_SIZES, (void*)ptr);
			}
		}

		public unsafe static void QueryContextAttributes(ref SafeCtxtHandle context, UlAttribute attribute, void* buffer)
		{
			int num = Secur32Dll.QueryContextAttributesA(ref context.Handle, (uint)attribute, buffer);
			if (num != 0)
			{
				throw new SspiException(num, "QueryContextAttributesA");
			}
		}

		public unsafe static SecurityStatus SafeQueryContextAttributes(ref SafeCtxtHandle context, out SecPkgContext_StreamSizes streamSizes)
		{
			return Sspi.SafeQueryContextAttributes(ref context, UlAttribute.SECPKG_ATTR_STREAM_SIZES, (void*)(&streamSizes));
		}

		public unsafe static SecurityStatus SafeQueryContextAttributes(ref SafeCtxtHandle context, out SecPkgContext_Sizes packageSizes)
		{
			return Sspi.SafeQueryContextAttributes(ref context, UlAttribute.SECPKG_ATTR_SIZES, (void*)(&packageSizes));
		}

		public unsafe static SecurityStatus SafeQueryContextAttributes(ref SafeCtxtHandle context, out string name)
		{
			SecPkgContext_Names[] array = new SecPkgContext_Names[1];
			fixed (IntPtr* ptr = array)
			{
				SecurityStatus result = Sspi.SafeQueryContextAttributes(ref context, UlAttribute.SECPKG_ATTR_NAMES, (void*)ptr);
				name = Marshal.PtrToStringAnsi(array[0].sUserName);
				Secur32Dll.FreeContextBuffer(array[0].sUserName);
				return result;
			}
		}

		public unsafe static SecurityStatus SafeQueryContextAttributes(ref SafeCtxtHandle context, UlAttribute attribute, void* buffer)
		{
			SecurityStatus result;
			try
			{
				int error = Secur32Dll.QueryContextAttributesA(ref context.Handle, (uint)attribute, buffer);
				result = Sspi.Convert(error);
			}
			catch
			{
				result = (SecurityStatus)4294967295u;
			}
			return result;
		}

		public static SecurityStatus SafeMakeSignature(SafeCtxtHandle context, ref SecBufferDescEx message, int sequence)
		{
			SecurityStatus result;
			try
			{
				message.Pin();
				int error = Secur32Dll.MakeSignature(ref context.Handle, 0, ref message.SecBufferDesc, sequence);
				result = Sspi.Convert(error);
			}
			catch
			{
				result = (SecurityStatus)4294967295u;
			}
			finally
			{
				message.Free();
			}
			return result;
		}

		public static SecurityStatus SafeVerifySignature(SafeCtxtHandle context, ref SecBufferDescEx message, int sequence)
		{
			SecurityStatus result;
			try
			{
				message.Pin();
				int num;
				int error = Secur32Dll.VerifySignature(ref context.Handle, ref message.SecBufferDesc, sequence, out num);
				result = Sspi.Convert(error);
			}
			catch
			{
				result = (SecurityStatus)4294967295u;
			}
			finally
			{
				message.Free();
			}
			return result;
		}

		public static SecurityStatus Convert(int error)
		{
			if (Enum.IsDefined(typeof(SecurityStatus), (uint)error))
			{
				return (SecurityStatus)error;
			}
			return (SecurityStatus)4294967295u;
		}
	}
}

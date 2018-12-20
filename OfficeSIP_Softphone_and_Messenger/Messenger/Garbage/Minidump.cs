// Copyright (C) 2010 OfficeSIP Communications
// This source is subject to the GNU General Public License.
// Please see Notice.txt for details.

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Messenger
{
	class Minidump
	{
		internal enum MINIDUMP_TYPE
		{
			MiniDumpNormal = 0x00000000,
			MiniDumpWithDataSegs = 0x00000001,
			MiniDumpWithFullMemory = 0x00000002,
			MiniDumpWithHandleData = 0x00000004,
			MiniDumpFilterMemory = 0x00000008,
			MiniDumpScanMemory = 0x00000010,
			MiniDumpWithUnloadedModules = 0x00000020,
			MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
			MiniDumpFilterModulePaths = 0x00000080,
			MiniDumpWithProcessThreadData = 0x00000100,
			MiniDumpWithPrivateReadWriteMemory = 0x00000200,
			MiniDumpWithoutOptionalData = 0x00000400,
			MiniDumpWithFullMemoryInfo = 0x00000800,
			MiniDumpWithThreadInfo = 0x00001000,
			MiniDumpWithCodeSegs = 0x00002000
		}

		[DllImport("dbghelp.dll")]
		static extern bool MiniDumpWriteDump(
			IntPtr hProcess,
			Int32 ProcessId,
			IntPtr hFile,
			MINIDUMP_TYPE DumpType,
			IntPtr ExceptionParam,
			IntPtr UserStreamParam,
			IntPtr CallackParam);

		
		public static void MiniDumpToFile(String fileName)
		{
			FileStream fileStream = null;

			if (File.Exists(fileName))
				fileStream = File.Open(fileName, FileMode.Append);
			else
				fileStream = File.Create(fileName);
			
			Process thisProcess = Process.GetCurrentProcess();
			
			MiniDumpWriteDump(thisProcess.Handle, thisProcess.Id,
				//MiniDumpWithFullMemory, MiniDumpNormal
				fileStream.SafeFileHandle.DangerousGetHandle(), MINIDUMP_TYPE.MiniDumpNormal | MINIDUMP_TYPE.MiniDumpCallback,
				IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			
			fileStream.Close();
		}
	}
}

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.Ssp
{
	public class SecBufferDescEx
	{
		internal SecBufferDesc SecBufferDesc;

		internal SecBuffer[] SecBuffers;

		private GCHandle[] Handles;

		private GCHandle DescHandle;

		public SecBufferEx[] Buffers;

		public SecBufferDescEx(SecBufferEx[] buffers)
		{
			this.SecBufferDesc.ulVersion = 0;
			this.SecBufferDesc.cBuffers = 0;
			this.SecBufferDesc.pBuffers = IntPtr.Zero;
			this.Handles = null;
			this.SecBuffers = null;
			this.Buffers = buffers;
		}

		public int GetBufferIndex(BufferType type, int from)
		{
			for (int i = from; i < this.Buffers.Length; i++)
			{
				if (this.Buffers[i].BufferType == type)
				{
					return i;
				}
			}
			return -1;
		}

		internal void Pin()
		{
			if (this.SecBuffers == null || this.SecBuffers.Length != this.Buffers.Length)
			{
				this.SecBuffers = new SecBuffer[this.Buffers.Length];
				this.Handles = new GCHandle[this.Buffers.Length];
			}
			for (int i = 0; i < this.Buffers.Length; i++)
			{
				if (this.Buffers[i].Buffer != null)
				{
					this.Handles[i] = GCHandle.Alloc(this.Buffers[i].Buffer, GCHandleType.Pinned);
				}
				this.SecBuffers[i].BufferType = (int)this.Buffers[i].BufferType;
				this.SecBuffers[i].cbBuffer = this.Buffers[i].Size;
				if (this.Buffers[i].Buffer == null)
				{
					this.SecBuffers[i].pvBuffer = IntPtr.Zero;
				}
				else
				{
					this.SecBuffers[i].pvBuffer = this.AddToPtr(this.Handles[i].AddrOfPinnedObject(), this.Buffers[i].Offset);
				}
			}
			this.DescHandle = GCHandle.Alloc(this.SecBuffers, GCHandleType.Pinned);
			this.SecBufferDesc.ulVersion = 0;
			this.SecBufferDesc.cBuffers = this.SecBuffers.Length;
			this.SecBufferDesc.pBuffers = this.DescHandle.AddrOfPinnedObject();
		}

		internal void Free()
		{
			object buffer = this.Buffers[0].Buffer;
			IntPtr begin = this.Handles[0].AddrOfPinnedObject();
			for (int i = 0; i < this.Buffers.Length; i++)
			{
				this.Buffers[i].BufferType = (BufferType)this.SecBuffers[i].BufferType;
				this.Buffers[i].Size = this.SecBuffers[i].cbBuffer;
				if (this.Buffers[i].Size == 0 || this.Buffers[i].BufferType == BufferType.SECBUFFER_VERSION)
				{
					this.Buffers[i].Buffer = null;
					this.Buffers[i].Offset = 0;
				}
				else
				{
					this.Buffers[i].Buffer = buffer;
					if (this.SecBuffers[i].pvBuffer != IntPtr.Zero)
					{
						this.Buffers[i].Offset = this.SubPtr(begin, this.SecBuffers[i].pvBuffer);
					}
				}
			}
			for (int j = 0; j < this.Buffers.Length; j++)
			{
				if (this.Handles[j].IsAllocated)
				{
					this.Handles[j].Free();
				}
			}
			this.DescHandle.Free();
		}

		private int SubPtr(IntPtr begin, IntPtr current)
		{
			return (int)((long)current - (long)begin);
		}

		private IntPtr AddToPtr(IntPtr begin, int offset)
		{
			return (IntPtr)((long)begin + (long)offset);
		}
	}
}

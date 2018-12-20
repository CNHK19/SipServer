using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace SocketServers
{
	public class ServerAsyncEventArgs : EventArgs, ILockFreePoolItem, IDisposable
	{
		internal delegate void CompletedEventHandler(Socket socket, ServerAsyncEventArgs e);

		public const int AnyNewConnectionId = -1;

		public const int AnyConnectionId = -2;

		public const int DefaultSize = 2048;

		public static int DefaultOffsetOffset;

		internal int SequenceNumber;

		private bool isPooled;

		private int count;

		private int offsetOffset;

		private int bytesTransferred;

		private ArraySegment<byte> segment;

		private SocketAsyncEventArgs socketArgs;

		internal ServerAsyncEventArgs.CompletedEventHandler Completed;

		bool ILockFreePoolItem.IsPooled
		{
			set
			{
				this.isPooled = value;
			}
		}

		public int UserTokenForSending
		{
			get;
			set;
		}

		public int UserTokenForSending2
		{
			get;
			set;
		}

		internal Socket AcceptSocket
		{
			get
			{
				return this.socketArgs.AcceptSocket;
			}
			set
			{
				this.socketArgs.AcceptSocket = value;
			}
		}

		public SocketError SocketError
		{
			get
			{
				return this.socketArgs.SocketError;
			}
			internal set
			{
				this.socketArgs.SocketError = value;
			}
		}

		public bool DisconnectReuseSocket
		{
			get
			{
				return this.socketArgs.DisconnectReuseSocket;
			}
			set
			{
				this.socketArgs.DisconnectReuseSocket = value;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.socketArgs.RemoteEndPoint as IPEndPoint;
			}
			set
			{
				if (!(this.socketArgs.RemoteEndPoint as IPEndPoint).Equals(value))
				{
					(this.socketArgs.RemoteEndPoint as IPEndPoint).Address = new IPAddress(value.Address.GetAddressBytes());
					(this.socketArgs.RemoteEndPoint as IPEndPoint).Port = value.Port;
				}
			}
		}

		public ServerEndPoint LocalEndPoint
		{
			get;
			set;
		}

		public int ConnectionId
		{
			get;
			set;
		}

		public int OffsetOffset
		{
			get
			{
				return this.offsetOffset;
			}
			set
			{
				this.offsetOffset = value;
			}
		}

		public int Offset
		{
			get
			{
				return this.segment.Offset + this.offsetOffset;
			}
			set
			{
				this.offsetOffset = value - this.segment.Offset;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
			set
			{
				this.count = value;
			}
		}

		public int BytesTransferred
		{
			get
			{
				return this.bytesTransferred;
			}
			set
			{
				this.bytesTransferred = value;
			}
		}

		public byte[] Buffer
		{
			get
			{
				if (this.offsetOffset + this.count > this.segment.Count)
				{
					this.ReAllocateBuffer(false);
				}
				return this.segment.Array;
			}
		}

		public int MinimumRequredOffsetOffset
		{
			get
			{
				if (this.LocalEndPoint == null)
				{
					throw new ArgumentException("You MUST set LocalEndPoint before this action.");
				}
				if (this.LocalEndPoint.Protocol != ServerProtocol.Tls)
				{
					return 0;
				}
				return 256;
			}
		}

		public ArraySegment<byte> BufferSegment
		{
			get
			{
				return this.segment;
			}
		}

		public ArraySegment<byte> TransferredData
		{
			get
			{
				return new ArraySegment<byte>(this.Buffer, this.Offset, this.BytesTransferred);
			}
		}

		public ArraySegment<byte> IncomingData
		{
			get
			{
				return this.TransferredData;
			}
		}

		public ArraySegment<byte> OutgoingData
		{
			get
			{
				return new ArraySegment<byte>(this.Buffer, this.Offset, this.Count);
			}
		}

		public ServerAsyncEventArgs()
		{
			this.socketArgs = new SocketAsyncEventArgs
			{
				RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0),
				UserToken = this
			};
			this.socketArgs.Completed += new EventHandler<SocketAsyncEventArgs>(ServerAsyncEventArgs.SocketArgs_Completed);
			this.SetDefaultValue();
		}

		public void Dispose()
		{
			if (this.isPooled)
			{
				BufferManager.Free(ref this.segment);
				this.socketArgs.Dispose();
				return;
			}
			EventArgsManager.Put(this);
		}

		public void SetDefaultValue()
		{
			this.ConnectionId = -1;
			this.LocalEndPoint = null;
			this.Completed = null;
			this.AcceptSocket = null;
			if (this.segment.Array != null && this.segment.Count != 2048)
			{
				BufferManager.Free(ref this.segment);
			}
			this.offsetOffset = ServerAsyncEventArgs.DefaultOffsetOffset;
			this.count = 2048 - ServerAsyncEventArgs.DefaultOffsetOffset;
			this.bytesTransferred = 0;
			this.UserTokenForSending = 0;
		}

		public ServerAsyncEventArgs CreateDeepCopy()
		{
			ServerAsyncEventArgs serverAsyncEventArgs = EventArgsManager.Get();
			serverAsyncEventArgs.CopyAddressesFrom(this);
			serverAsyncEventArgs.offsetOffset = this.offsetOffset;
			serverAsyncEventArgs.count = this.count;
			serverAsyncEventArgs.AllocateBuffer();
			serverAsyncEventArgs.bytesTransferred = this.bytesTransferred;
			serverAsyncEventArgs.UserTokenForSending = this.UserTokenForSending;
			System.Buffer.BlockCopy(this.Buffer, this.Offset, serverAsyncEventArgs.Buffer, serverAsyncEventArgs.Offset, serverAsyncEventArgs.Count);
			return serverAsyncEventArgs;
		}

		public static implicit operator SocketAsyncEventArgs(ServerAsyncEventArgs serverArgs)
		{
			if (serverArgs.Count > 0)
			{
				serverArgs.AllocateBuffer();
				serverArgs.socketArgs.SetBuffer(serverArgs.Buffer, serverArgs.Offset, serverArgs.Count);
			}
			else
			{
				serverArgs.socketArgs.SetBuffer(null, -1, -1);
			}
			return serverArgs.socketArgs;
		}

		public void CopyAddressesFrom(ServerAsyncEventArgs e)
		{
			this.ConnectionId = e.ConnectionId;
			this.LocalEndPoint = e.LocalEndPoint;
			this.RemoteEndPoint = e.RemoteEndPoint;
		}

		public void CopyAddressesFrom(BaseConnection c)
		{
			this.ConnectionId = c.Id;
			this.LocalEndPoint = c.LocalEndPoint;
			this.RemoteEndPoint = c.RemoteEndPoint;
		}

		public void SetAnyRemote(AddressFamily family)
		{
			if (family == AddressFamily.InterNetwork)
			{
				this.RemoteEndPoint.Address = IPAddress.Any;
			}
			else
			{
				this.RemoteEndPoint.Address = IPAddress.IPv6Any;
			}
			this.RemoteEndPoint.Port = 0;
		}

		public void SetMaxCount()
		{
			this.count = this.segment.Count - this.offsetOffset;
		}

		public void AllocateBuffer()
		{
			this.ReAllocateBuffer(false);
		}

		public void AllocateBuffer(int applicationOffsetOffset, int count)
		{
			this.OffsetOffset = this.MinimumRequredOffsetOffset + applicationOffsetOffset;
			this.Count = count;
			this.ReAllocateBuffer(false);
		}

		public void ReAllocateBuffer(bool keepData)
		{
			if (this.offsetOffset + this.count > this.segment.Count)
			{
				ArraySegment<byte> buffer = BufferManager.Allocate(this.offsetOffset + this.count);
				if (keepData && this.segment.IsValid())
				{
					System.Buffer.BlockCopy(this.segment.Array, this.segment.Offset, buffer.Array, buffer.Offset, this.segment.Count);
				}
				this.AttachBuffer(buffer);
			}
		}

		public void FreeBuffer()
		{
			BufferManager.Free(ref this.segment);
			this.Count = 0;
		}

		public void BlockCopyFrom(ArraySegment<byte> data)
		{
			if (data.Count > this.Count)
			{
				throw new ArgumentOutOfRangeException("BlockCopyFrom: data.Count > Count");
			}
			System.Buffer.BlockCopy(data.Array, data.Offset, this.Buffer, this.Offset, data.Count);
		}

		public void BlockCopyFrom(int offsetOffset, ArraySegment<byte> data)
		{
			if (data.Count > this.Count)
			{
				throw new ArgumentOutOfRangeException("BlockCopyFrom: data.Count > Count");
			}
			System.Buffer.BlockCopy(data.Array, data.Offset, this.Buffer, this.Offset + offsetOffset, data.Count);
		}

		public void AttachBuffer(ArraySegment<byte> buffer)
		{
			BufferManager.Free(this.segment);
			this.segment = buffer;
		}

		public void AttachBuffer(StreamBuffer buffer)
		{
			this.OffsetOffset = 0;
			this.BytesTransferred = buffer.BytesTransferred;
			this.AttachBuffer(buffer.Detach());
			this.Count = this.segment.Count;
		}

		public ArraySegment<byte> DetachBuffer()
		{
			ArraySegment<byte> result = this.segment;
			this.segment = default(ArraySegment<byte>);
			this.count = 2048;
			this.offsetOffset = 0;
			this.bytesTransferred = 0;
			return result;
		}

		internal void OnCompleted(Socket socket)
		{
			if (this.Completed != null)
			{
				this.Completed(socket, this);
			}
		}

		private static void SocketArgs_Completed(object sender, SocketAsyncEventArgs e)
		{
			ServerAsyncEventArgs serverAsyncEventArgs = e.UserToken as ServerAsyncEventArgs;
			serverAsyncEventArgs.bytesTransferred = e.BytesTransferred;
			serverAsyncEventArgs.Completed(sender as Socket, serverAsyncEventArgs);
		}

		[Conditional("DEBUG")]
		internal void ValidateBufferSettings()
		{
			if (this.Offset < this.segment.Offset)
			{
				throw new ArgumentOutOfRangeException("Offset is below than segment.Offset value");
			}
			if (this.OffsetOffset >= this.segment.Count)
			{
				throw new ArgumentOutOfRangeException("OffsetOffset is bigger than segment.Count");
			}
			if (this.BytesTransferred >= this.segment.Count)
			{
				throw new ArgumentOutOfRangeException("BytesTransferred is bigger than segment.Count");
			}
			if (this.OffsetOffset + this.Count > this.segment.Count)
			{
				throw new ArgumentOutOfRangeException("Invalid buffer settings: OffsetOffset + Count is bigger than segment.Count");
			}
		}

		[Conditional("EVENTARGS_TRACING")]
		public void Trace()
		{
		}

		[Conditional("EVENTARGS_TRACING")]
		public void ResetTracing()
		{
		}

		public string GetTracingPath()
		{
			return "NO TRACING";
		}
	}
}
